using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace NeoContract
{
    public class StandardToken : SmartContract
    {
        //public static event Action<byte[], byte[], BigInteger> TokenPreSaleTransfer;

        public static string name = "New Token";
        public static string symbol = "SP";
        public static int decimals = 18;
        public const ulong factor = 100000;
        public static ulong totalSupply = 75;  
        public static void Main(string operation,params object[] args)
        {
            if(operation=="transferFrom")
            {
                transferFrom((byte[])args[0],(byte[])args[1],(int)args[2]);
            }
            if(operation=="approve")
            {
                approve((byte[]) args[0],(byte[]) args[1],(BigInteger)args[2]);
            }
        }

        public static Boolean transferFrom(byte[] _from, byte[] _to, BigInteger _value)
        {
            if (_to != new byte[] { 0 })
            {
                BigInteger transferable =new BigInteger(Storage.Get(Storage.CurrentContext, _from.Concat(_to)));
                BigInteger fromValue = new BigInteger(Storage.Get(Storage.CurrentContext, _from));
                BigInteger toValue = new BigInteger(Storage.Get(Storage.CurrentContext, _to));

                if (_value <=transferable)
                {
                    if (_value <= fromValue)
                    {
                        Storage.Put(Storage.CurrentContext, _from.Concat(_to), transferable - _value);
                        Storage.Put(Storage.CurrentContext, _from,fromValue-_value);
                        Storage.Put(Storage.CurrentContext, _to, toValue - _value);
                        //Event for transfering
                    }
                }
            }
            return false;
        }

        public static Boolean approve(byte[] _spender,byte[] _to, BigInteger _value)
        {
            Storage.Put(Storage.CurrentContext,_spender.Concat(_to),_value);
            //Event for approval
            return true;
        }

        public static BigInteger Allowance(byte[] from, byte[] to)
        {
            return new BigInteger(Storage.Get(Storage.CurrentContext, from.Concat( to)));
        }

        public static Boolean IncreaseApproval(byte[] originator, byte[] _spender, BigInteger _addedValue)
        {
            BigInteger value = new BigInteger(Storage.Get(Storage.CurrentContext, originator.Concat(_spender)));
            value = value+_addedValue;
            Storage.Put(Storage.CurrentContext, originator.Concat(_spender), value);
            return true;
        }
       
        public Boolean DecreaseApproval(byte[] originator, byte[] _spender, BigInteger _subtractedValue)
        {
            BigInteger oldValue = new BigInteger(Storage.Get(Storage.CurrentContext, originator.Concat(_spender)));
            if (_subtractedValue> oldValue)
            {
                Storage.Put(Storage.CurrentContext, originator.Concat(_spender), BigInteger.Zero);
                return true;
            }
            else
            {
                Storage.Put(Storage.CurrentContext, originator.Concat(_spender), oldValue-_subtractedValue);
                return true;
            }
        }
    }
}
