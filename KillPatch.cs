using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using GameNetcodeStuff;
using System.Reflection;

namespace Lethal_Company
{
    [HarmonyPatch(typeof(PlayerControllerB), "KillPlayerServerRpc")]

    class KillPatch
    {
        public static void KillPlayerServerRpcPrefix(PlayerControllerB __instance, int playerId, bool spawnBody, Vector3 BodyVelocity, int causeOfDeath, int deathAnimation)
        {
            MethodInfo methodInfo = typeof(PlayerControllerB).GetMethod("KillPlayerServerRpc", BindingFlags.Instance| BindingFlags.NonPublic);
            {
                if (methodInfo != null) 
                {
                    methodInfo.Invoke(__instance, new object[] { playerId, spawnBody, BodyVelocity, causeOfDeath, deathAnimation });
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB),"KillPlayerClientRpc")]
    class KillPlayerClient
    {
        public static void KillPlayerClientRpcPrefix(PlayerControllerB __instance, int playerId, bool spawnBody, Vector3 BodyVelocity, int causeOfDeath, int deathAnimation)
        {
            MethodInfo methodInfo = typeof(PlayerControllerB).GetMethod("KillPlayerClientRpcPrefix", BindingFlags.Instance | BindingFlags.NonPublic);
            {
                if (methodInfo != null)
                {
                    methodInfo.Invoke(__instance, new object[] { playerId, spawnBody, BodyVelocity, causeOfDeath, deathAnimation });
                    return;
                }
            }
        }
    }
}
