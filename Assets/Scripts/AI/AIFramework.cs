using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {

	public abstract class Condition : ScriptableObject {

        public abstract bool Evaluate (AIBrain brain);

    }

    public abstract class Action : ScriptableObject {

        public abstract void Execute (AIBrain brain);

    }

    [System.Serializable]
    public class Response {

        public Condition condition;
        public Action action;

    }

    [System.Serializable]
    public class Preference {

        public Condition condition;
        public AIBehaviour behaviour;

    }

}
