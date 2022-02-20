using Stratis.SmartContracts;
//^ using Microsoft.Contracts;


public class Player : SmartContract
{
    //^ [NotDelayed]
	public Player(ISmartContractState state, Address player, Address opponent, string gameName)
        : base(state)
    {
        PlayerAddress = player;
        Opponent = opponent;
        GameName = gameName;
        State = (uint)StateType.Provisioned;
    }

    public enum StateType : uint
    {
        Provisioned = 0,
        SentPing = 1,
        ReceivedPing = 2,
        Finished = 3
    }
	//@ public string Foo;
    public uint State
    {
        get => PersistentState.GetUInt32(nameof(State));
        private set => PersistentState.SetUInt32(nameof(State), value);
    }

    public Address Opponent
    {
        get => PersistentState.GetAddress(nameof(Opponent));
        private set => PersistentState.SetAddress(nameof(Opponent), value);
    }

    public Address PlayerAddress
    {
        get => PersistentState.GetAddress(nameof(PlayerAddress));
        private set => PersistentState.SetAddress(nameof(PlayerAddress), value);
    }

    public string GameName
    {
        get => PersistentState.GetString(nameof(GameName));
        private set => PersistentState.SetString(nameof(GameName), value);
    }

    public uint PingsSent
    {
        get => PersistentState.GetUInt32(nameof(PingsSent));
        private set => PersistentState.SetUInt32(nameof(PingsSent), value);
    }

    public uint PingsReceived
    {
        get => PersistentState.GetUInt32(nameof(PingsReceived));
        private set => PersistentState.SetUInt32(nameof(PingsReceived), value);
    }

    public void ReceivePing()
    {
        Assert(Message.Sender == Opponent);
        Assert(State == (uint)StateType.SentPing || State == (uint)StateType.Provisioned);

        State = (uint)StateType.ReceivedPing;

        // We want to overflow the counter here.
        unchecked
        {
            PingsReceived += 1;
        }
    }

    public void SendPing()
    {
        Assert(Message.Sender == PlayerAddress);     
        Assert(State == (uint)StateType.ReceivedPing || State == (uint)StateType.Provisioned);

        ITransferResult isFinishedResult = Call(Opponent, 0, nameof(Player.IsFinished));

        Assert(isFinishedResult.Success);

        // End the game if the opponent is finished.
        if ((bool)isFinishedResult.ReturnValue)
        {
            State = (uint)StateType.Finished;
            return;
        }

        // We want to overflow the counter here.
        unchecked
        {
            PingsSent += 1;
        }

        ITransferResult callResult = Call(Opponent, 0, nameof(Player.ReceivePing));
        Assert(callResult.Success);
        State = (uint)StateType.SentPing;
    }

    public bool IsFinished()
    {
        return State == (uint)StateType.Finished;
    }

    public void FinishGame()
    {
        Assert(Message.Sender == PlayerAddress);
        State = (uint)StateType.Finished;
    }
}

[Deploy]
public class Starter : SmartContract
{
    public Starter(ISmartContractState state)
        : base(state)
    {
        
    }

    /// <summary>
    /// Creates two contracts that can ping/pong back and forth between each other up to <see cref="maxPingPongTimes"/>.
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    /// <param name="gameName"></param>
    /// <param name="maxPingPongTimes"></param>
    public void StartGame(Address player1, Address player2, string gameName)
    {
        ICreateResult player1CreateResult = Create<Player>(0, new object[] { player1, player2, gameName }, 0);

        Assert(player1CreateResult.Success);

        ICreateResult player2CreateResult = Create<Player>(0, new object[] { player2, player1, gameName }, 0);

        Assert(player2CreateResult.Success);

        GameCreated gc = new GameCreated();
        gc.Player1Contract = player1CreateResult.NewContractAddress;
        gc.Player2Contract = player2CreateResult.NewContractAddress;
        gc.GameName = gameName;
        Log<GameCreated>(gc);
    }

    public struct GameCreated
    {
        [Index]
        public Address Player1Contract;

        [Index]
        public Address Player2Contract;

        public string GameName;
    }
}