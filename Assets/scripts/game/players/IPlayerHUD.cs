using Assets.scripts.game.weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.game.players
{
    public interface IPlayerHUD
    {

        void GameHasStarted();

        void SetHealth(float health);

        void SetWeapon(TheWeaponEnum weapon);

        

        void GameOver(bool won);


        void Shoot();

    }
}
