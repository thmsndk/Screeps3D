using Screeps3D.RoomObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Screeps3D.Tools.Selection
{
    public class SelectionRoomObjectButton<T> : IRoomObjectPanelButton<T> where T:IRoomObject
    {
        public string Text { get; set; }
        public Action<T> action { get; set; }

        public SelectionRoomObjectButton(string text, Action<T> action)
        {
            this.Text = text;
            this.action = action;
        }

        public void OnClick(T roomObject)
        {
            this.action(roomObject);
        }
    }
}
