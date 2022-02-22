using Stratis.SmartContracts;
//@ using Microsoft.Contracts;

public class HelloBlockchain : SmartContract
{
	public HelloBlockchain(ISmartContractState state, string message)
		: base(state)
	{

		/*
		Requestor = Message.Sender;
		RequestMessage = message;
		State = (uint)StateType.Request;
		*/
		State4 = state.PersistentState;
	}
	//@ [Rep]
	public HelloState State2 = new HelloState();
	
	public IPersistentState State4;
	public enum StateType : uint
	{
		Request = 0,
		Respond = 1
	}

	public void Hello()
	
	{
		//ContractState = 0;
		
		uint gg = ContractState;
		
	}

	//@ [Pure]
	public uint Foo()
    {
		return 0;
    }

	
	public uint ContractState
	{
		
		
		get
		
		{
			
				return State2.H3();
			
			
			//return Foo();
		}



	}
	/*
	public Address Requestor
	{
		get => PersistentState.GetAddress(nameof(Requestor));
		private set => PersistentState.SetAddress(nameof(Requestor), value);
	}

	public Address Responder
	{
		get => PersistentState.GetAddress(nameof(Responder));
		private set => PersistentState.SetAddress(nameof(Responder), value);
	}

	public string RequestMessage
	{
		get => PersistentState.GetString(nameof(RequestMessage));
		private set => PersistentState.SetString(nameof(RequestMessage), value);
	}

	public string ResponseMessage
	{
		get => PersistentState.GetString(nameof(ResponseMessage));
		private set => PersistentState.SetString(nameof(ResponseMessage), value);
	}

	public void SendRequest(string requestMessage)
	{
		Assert(Message.Sender == Requestor, "Sender is not requestor");

		RequestMessage = requestMessage;
		State = (uint)StateType.Request;
	}

	public void SendResponse(string responseMessage)
	{
		Responder = Message.Sender;

		ResponseMessage = responseMessage;
		State = (uint)StateType.Respond;
	}
	*/
}
