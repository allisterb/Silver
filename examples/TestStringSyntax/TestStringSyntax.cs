using Stratis.SmartContracts;

public class TestStringSyntax : SmartContract
{
	const int MAX_LEN = 100;

	public int foo = 0;
	public TestStringSyntax(ISmartContractState state, string message)
		: base(state)
	{
		Requestor = Message.Sender;
		RequestMessage = message;
		ContractState = (uint)StateType.Request;
	}

	public enum StateType : uint
	{
		Request = 0,
		Respond = 1
	}

	public uint ContractState
	{
		get => State.GetUInt32(nameof(State));
		private set => State.SetUInt32(nameof(State), value);
	}

	public Address Requestor
	{
		get => State.GetAddress(nameof(Requestor));
		private set => State.SetAddress(nameof(Requestor), value);
	}

	public Address Responder
	{
		get => State.GetAddress(nameof(Responder));
		private set => State.SetAddress(nameof(Responder), value);
	}

	public string RequestMessage
	{
		get => State.GetString(nameof(RequestMessage));
		private set => State.SetString(nameof(RequestMessage), value);
	}

	public string ResponseMessage
	{
		get => State.GetString(nameof(ResponseMessage));
		private set => State.SetString(nameof(ResponseMessage), value);
	}

	public void SendRequest(string requestMessage)
	{
		Assert(Message.Sender == Requestor, "Sender is not requestor");

		RequestMessage = requestMessage;
		ContractState = (uint)StateType.Request;
	}

	public void SendResponse(string responseMessage)
	{
		Responder = Message.Sender;

		ResponseMessage = responseMessage;
		ContractState = (uint)StateType.Respond;
	}
	
	public void Test1(string message)
	{
		// int MAX_LEN = 100;
		Assert(message.Length <= MAX_LEN, $"Cannot handle more than {MAX_LEN} characters");
	}
	
	public void Test2(string message)
	{
		// int MAX_LEN = 100;
		Assert(message.Length <= MAX_LEN, string.Format("Cannot handle more than {0} characters.", MAX_LEN));
	}

	public void Test3(string message)
	{
		// int MAX_LEN = 100;
		Assert(message.Length <= MAX_LEN, "Cannot handle more than "+ MAX_LEN + "characters.");
	}

	public struct TestStruct1
	{
		[Index]
		public Address Address1;

		[Index]
		public Address Address2;

		public string String1;
	}
}
