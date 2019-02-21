using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT {

	/// <summary>
	/// BTComposite is the base class for composite nodes.
	/// </summary>
	public class BTComposite : BTNode
    {
		[SerializeField][HideInInspector]
		private List<BTNode> mChildNodes = new List<BTNode>();

		public List<BTNode> ChildNodes
        {
			get
            {
				return mChildNodes;
			}
		}


		public override void Activate (BTDatabase database)
        {
			base.Activate (database);

            for (int i = 0, max = ChildNodes.Count; i < max; ++i)
            {
                BTNode tmpNode = ChildNodes[i];

                if (null != tmpNode)
                {
                    tmpNode.Activate(database);
                }
            }
        }

        public override void Clear()
        {
            base.Clear();

            for (int i = 0, max = ChildNodes.Count; i < max; ++i)
            {
                BTNode tmpNode = ChildNodes[i];

                if (null != tmpNode)
                {
                    tmpNode.Clear();
                }
            }
        }

        public void AddChild (BTNode node)
        {
			if (node != null)
            {
				ChildNodes.Add(node);
			}
		}

		public void RemoveChild (BTNode node)
        {
			ChildNodes.Remove(node);
		}
	}

}