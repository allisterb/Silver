using Stratis.SmartContracts;
//@ using Microsoft.Contracts;

public class HelloBlockchain : SmartContract
{
	
	//@ [NotDelayed]
	public HelloBlockchain(ISmartContractState state, string message)
		:base(state)
	{
		
		
		/*

		State = (uint)StateType.Request;
		*/



	}

	
	public enum StateType : uint
	{
		Request = 0,
		Respond = 1
	}

	//public StateType StateType { get; set; }


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
		//ContractState = (uint)StateType.Request;
	}

	
	public void SendResponse(string responseMessage)
	{
		Responder = Message.Sender;
		ResponseMessage = responseMessage;
		//ContractState = (uint)StateType.Respond;
		
	}
	
	
}
