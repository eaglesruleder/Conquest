using UnityEngine;
using System.Collections;

[System.Serializable]
public class Technology : MonoBehaviour {

	public string FactionName;

	public TechNode baseNode;
	public TechNode leftNode1;
	public TechNode leftNode2;
	public TechNode rightNode1;
	public TechNode rightNode2;
	public TechNode topNode;

	//Used for quick reference
	TechNode[] leftNodes;
	TechNode[] rightNodes;

	public void Initialise()
		{
		leftNodes = new TechNode[]{baseNode, leftNode1, leftNode2, topNode};
		rightNodes = new TechNode[]{baseNode, rightNode1, rightNode2, topNode};
		}
	
	public TechNode getNode(TechLocation location)
		{
		TechNode tempReturn;
		if(location.leftSide)
			tempReturn = leftNodes[location.height];
		else
			tempReturn = rightNodes[location.height];
		return tempReturn;
		}

	public int getHighestNode()
		{
		int height = 0;
		for (int i = 0; i < leftNodes.Length; i++)
			{
			if(leftNodes[i].ReturnLevels(true, false, false) > 0 || rightNodes[i].ReturnLevels(true, false, false) > 0)
				height = i;
			}
		return height;
		}

/* - - - - - */
	[System.Serializable]
	//A small subclass location object
	public class TechLocation
		{
		public int height = 0;
		public bool leftSide = false;
		
		public bool isBase = false;
		public bool isLeft = false;
		public bool isRight = false;
		
		public TechLocation(int Height, bool LeftSide, bool IsBase, bool IsLeft, bool IsRight)
			{
			height = Height;
			leftSide = LeftSide;
			isBase = IsBase;
			isLeft = IsLeft;
			isRight = IsRight;
			}
		}

	[System.Serializable]
	//A small subclass node for the technology tree.
	public class TechNode
		{
		public string baseTitle;
		int baseLevel = 0;

		public string leftTitle;
		public float leftModifier;
		int leftLevel = 0;

		public string rightTitle;
		public float rightModifier;
		int rightLevel = 0;

		public void IncrementLevel(int NodeIncrememnt)
			{
			for(int i = 0; i < NodeIncrememnt; i++)
				{
				if(baseLevel < 10)
					baseLevel++;
				float rand = Random.value;
				if(leftLevel < 5 && rand < 0.5f)
					leftLevel++;
				else if (rightLevel < 5)
					rightLevel++;
				}
			}

		public int ReturnLevels(TechLocation Location)
			{
			int tempReturn = ReturnLevels(Location.isBase, Location.isLeft, Location.isRight);
			return tempReturn;
			}

		public int ReturnLevels(bool IncludeBase, bool LeftLevel, bool RightLevel)
			{
			int techValue = 0;
			
			if(IncludeBase)
				techValue += baseLevel;

			if(LeftLevel)
				techValue += leftLevel;

			if(RightLevel)
				techValue += rightLevel;

			return techValue;
			}
		
		public float ReturnBonus(TechLocation Location)
			{
			float tempReturn = 0f;
			if(Location.isLeft)
				tempReturn = ReturnBonus(Location.isLeft);
			else if (Location.isRight)
				tempReturn = ReturnBonus(Location.isRight);
			return tempReturn;
			}
		
		public float ReturnBonus(bool leftLevel)
			{
			int currTech = 0;
			float  modTech = 0f;
			if(leftLevel)
				{
				currTech = ReturnLevels(false, true, false);
				modTech = leftModifier;
				}
			else
				{
				currTech = ReturnLevels(false, false, true);
				modTech = rightModifier;
				}
			float bonus = modTech * currTech;
			return bonus;
			}
		}
}
