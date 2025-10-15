using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class FunctionHandler : ActionHandler
    {
        [SerializeField] protected string currentFunction;
        protected bool waitForEnd;
        protected float completeDelay;
        protected Dictionary<string, Action> FunctionMapping = new();
        
        
        public void ChooseFunction(string functionName, bool wait, float delay = 0f)
        {
            currentFunction = functionName;
            waitForEnd = wait;
            completeDelay = delay;
        }
    }
}