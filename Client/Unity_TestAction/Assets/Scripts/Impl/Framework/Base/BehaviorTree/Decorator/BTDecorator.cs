using UnityEngine;
using System.Collections;

namespace BT {

	/// <summary>
	/// BTDecorator is the base class for decorator nodes.
	/// It has and only has one child.
	/// Usually it is used to alter the behavior of its child.
	/// </summary>
	public class BTDecorator : BTNode
    {
		public BTNode ChildNode { get; set; }


		public BTDecorator (BTNode node)
        {
			this.ChildNode = node;
		}

		public override void Activate (BTDatabase database)
        {
			base.Activate (database);

			ChildNode.Activate(database);
		}

		public override void Clear ()
        {
			base.Clear ();

			ChildNode.Clear();
		}
	}

}