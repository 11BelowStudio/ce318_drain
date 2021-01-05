using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.scripts.game.players
{
    public interface IControlHealth
    {

        /// <summary>
        /// Starts health drain
        /// </summary>
        void WillYouStartTheDrainsPlease();


        /// <summary>
        /// Stops health drain
        /// </summary>
        void WillYouStopTheDrainsPlease();

        /// <summary>
        /// Whether or not contestant is dead
        /// </summary>
        /// <returns></returns>
        Boolean IsDed();
    }
}
