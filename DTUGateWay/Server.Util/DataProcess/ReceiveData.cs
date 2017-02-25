namespace Server.Util.DataProcess
{
    public class ReceiveData
    {
        public virtual bool parsePacket(byte[] packet, int offset, int length)
        {
            return true;
        }

        public virtual object returnObject()
        {
            return null;
        }
    }
}
