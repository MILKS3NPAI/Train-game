﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

class NPCScriptedActions
{
	public static UnityAction<Entity>[] enterActions = new UnityAction<Entity>[ConstantResources.ArraySize<ScriptedAction>()];
	public static UnityAction<Entity>[] updateActions = new UnityAction<Entity>[ConstantResources.ArraySize<ScriptedAction>()];
	public static UnityAction<Entity>[] fixedActions = new UnityAction<Entity>[ConstantResources.ArraySize<ScriptedAction>()];
	public static UnityAction<Entity>[] exitActions = new UnityAction<Entity>[ConstantResources.ArraySize<ScriptedAction>()];

	static bool initialized = false;

	public static void Initialize()
	{
		if (initialized)
		{
			return;
		}
		for (int i = 0; i < enterActions.Length; i++)
		{
			enterActions[i] = DoNothing;
			updateActions[i] = DoNothing;
			fixedActions[i] = DoNothing;
			exitActions[i] = DoNothing;
		}
		enterActions[(int)ScriptedAction.RESET_PATROL] = ResetPatrol;
		enterActions[(int)ScriptedAction.RESET_PATROL] = RunAndHide;
	}

	static void ResetPatrol(Entity iEntity)
	{
		NPC lNPC;
		if (iEntity is NPC)
		{
			lNPC = (NPC)iEntity;
		}
		else
		{
			return;
		}
		lNPC.ResetPatrol();
		lNPC.mAIState = AIState.PATROL;
	}

	static void RunAndHide(Entity iEntity)
	{
		NPC lNPC;
		if (iEntity is NPC)
		{
			lNPC = (NPC)iEntity;
		}
		else
		{
			return;
		}
		lNPC.mAIState = AIState.INACTIVE;
	}

	static void DoNothing(Entity iDummy) { }
}