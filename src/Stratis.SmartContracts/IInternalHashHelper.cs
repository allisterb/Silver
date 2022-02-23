using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IInternalHashHelper
  {
        [Pure]
        byte[] Keccak256(byte[] toHash);
  }
}
