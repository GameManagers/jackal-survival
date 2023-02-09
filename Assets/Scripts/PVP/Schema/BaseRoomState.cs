using Colyseus.Schema;

namespace WE.PVP.Schemas
{
    public class BaseRoomState : Schema
    {
        [Type(0, "number")]
        public float Time = 0;
    }
}
