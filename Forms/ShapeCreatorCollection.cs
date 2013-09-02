using System.Collections.ObjectModel;
using Nummite.Shapes;

namespace Nummite.Forms {
	class ShapeCreatorCollection : KeyedCollection<string, IShapeCreator>
	{
		public void AddRange (params IShapeCreator[] shapeCreators)
		{
			foreach (var shapeCreator in shapeCreators)
				Add (shapeCreator);
		}

		protected override string GetKeyForItem (IShapeCreator item)
		{
			return item.TypeName;
		}
	}
}
