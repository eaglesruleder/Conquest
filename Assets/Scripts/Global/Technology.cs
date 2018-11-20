using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Requires servere overhaul
[System.Serializable]
public class Technology : MonoBehaviour {

	public string FactionName;

	//Used for quick reference
	/*public TechNode[] nodes;

	public static TechInfluence Engine { get;}
	public static TechInfluence Damage { get;}
	public static TechInfluence Volley { get;}
	public static TechInfluence Shield { get;}
	public static TechInfluence Health { get;}*/

	/*internal TechNode GetNode(TechLocation location)
		{
		TechNode tempReturn;
		if(location.leftSide)
			tempReturn = leftNodes[location.height];
		else
			tempReturn = rightNodes[location.height];
		return tempReturn;
		}*/

	[System.Serializable]
	//A small subclass location object
	public class TechInfluence
	{
		public TechLocation loc;
		//public int loc = 0;
        public float influence = 0;
		public float bonus = 0;

        Technology tech;
        TechNode node;
		
		public void SetTechnology(Technology Tech)
		{
			tech = Tech;
		}

        public int Level
        {
            get
            {
                if (node == null) throw new UnityEngine.UnityException("TechLocation ReturnLevel without node");
                return node.Level(loc);
            }
        }

        public float Bonus
        {
            get
            {
                if (node == null) throw new UnityEngine.UnityException("TechLocation ReturnBonus without node");
                return node.Bonus(loc);
            }
        }
	}

	[System.Serializable]
	public struct TechLocation
	{
		public int height;
		public bool leftSide;
		public bool isLeft;
		public bool isRight;
	}

    [System.Serializable]
    //A small subclass node for the technology tree.
    internal class TechNode
    {
        public string baseTitle;
        int baseLevel = 0;

        public string leftTitle;
        public float leftModifier;
        int leftLevel = 0;

        public string rightTitle;
        public float rightModifier;
        int rightLevel = 0;

        internal void IncrementLevel(int NodeIncrememnt)
        {
            for (int i = 0; i < NodeIncrememnt; i++)
            {
                if (baseLevel < 5)
                {
                    baseLevel++;
                    float rand = Random.value;
                    if (rand < 0.5f)
                        leftLevel++;
                    else
                        rightLevel++;
                }
            }
        }

		internal int Level()
		{
			return baseLevel;
		}

        internal int Level(TechLocation Loc)
        {
            int techValue = baseLevel;

            if (Loc.isLeft)
            {
                techValue += leftLevel;
            }

            if (Loc.isRight)
            {
                techValue += rightLevel;
            }

            return techValue;
        }

        internal float Bonus(TechLocation Loc)
        {
            float result = 0f;

            if(Loc.isLeft)
            {
                result += leftLevel * leftModifier;
            }
            if(Loc.isRight)
            {
                result += rightLevel * rightModifier;
            }

            return result;
        }
    }
}
