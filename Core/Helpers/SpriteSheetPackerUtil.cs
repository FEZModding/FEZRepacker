using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

/*
 * Modified image packer from SpriteSheetPacker by Nick Gravelyn
 * including rectangle packer by Javier Arevalo
 *
 * The algorithm was modified for usage of regenerating sprite sheets
 * accurate to those included as AnimatedTextures in FEZ.
 * Following simplifications were applied:
 * - frames are processed sequentially, ordered by their index
 * - pack space is always a power of 2
 * - all frames are equal size
 */

namespace FEZRepacker.Core.Helpers
{
	public static class SpriteSheetPackerUtil
	{
		private const int InitialAtlasSize = 2048;
		
		public static void PackFrames(this AnimatedTexture tex, int padding)
		{
			tex.AtlasWidth = InitialAtlasSize;
			tex.AtlasHeight  = InitialAtlasSize;
			var lastAtlasWidth = InitialAtlasSize;
			var lastAtlasHeight = InitialAtlasSize;

			var shrinkVertical = false;

			while (true)
			{
				if (!tex.TryPackFramesInCurrentAtlasSize(padding))
				{
					if (shrinkVertical)
					{
						break;
					}

					shrinkVertical = true;
					tex.AtlasWidth += tex.FrameWidth + padding + padding;
					tex.AtlasHeight += tex.FrameHeight + padding + padding;
					continue;
				}

				tex.ResizeAtlasToSmallestFrameFit();

				if (!shrinkVertical)
				{
					tex.AtlasWidth -= padding;
				}
				tex.AtlasHeight -= padding;

				tex.ResizeAtlasToNextPowerOfTwo();
				
				if (tex.AtlasWidth == lastAtlasWidth && tex.AtlasHeight == lastAtlasHeight)
				{
					if (shrinkVertical)
					{
						break;
					}
					shrinkVertical = true;
				}

				lastAtlasWidth = tex.AtlasWidth;
				lastAtlasHeight = tex.AtlasHeight;

				if (!shrinkVertical)
				{
					tex.AtlasWidth -= tex.FrameWidth;
				}
				tex.AtlasHeight -= tex.FrameHeight;
			}
			
			tex.ResizeAtlasToSmallestFrameFit();
			tex.ResizeAtlasToNextPowerOfTwo();
		}

		private static bool TryPackFramesInCurrentAtlasSize(this AnimatedTexture tex, int padding)
		{
			ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(tex.AtlasWidth, tex.AtlasHeight);

			for (var i = 0; i < tex.Frames.Count; i++)
			{
				var frameRect = tex.Frames[i].Rectangle;
				if (!rectanglePacker.TryPack(frameRect, padding))
				{
					return false;
				}
				tex.Frames[i].Rectangle = frameRect;
			}

			return true;
		}

		private static void ResizeAtlasToSmallestFrameFit(this AnimatedTexture tex)
		{
			tex.AtlasWidth = tex.AtlasHeight = 0;
			foreach (var frame in tex.Frames)
			{
				tex.AtlasWidth = Math.Max(tex.AtlasWidth, frame.Rectangle.X + frame.Rectangle.Width);
				tex.AtlasHeight = Math.Max(tex.AtlasHeight, frame.Rectangle.Y + frame.Rectangle.Height);
			}
		}

		private static void ResizeAtlasToNextPowerOfTwo(this AnimatedTexture tex)
		{
			tex.AtlasWidth = NextPowerOfTwo(tex.AtlasWidth);
			tex.AtlasHeight = NextPowerOfTwo(tex.AtlasHeight);
		}
		
		private static int NextPowerOfTwo(int num)
		{
			const int bitsInInt = sizeof(int) * 8;
			
			num--;
			for (var i = 1; i < bitsInInt; i <<= 1)
			{
				num |= num >> i;
			}
			return num + 1;
		}
		
		#region ArevaloRectanglePacker
		 
		private class ArevaloRectanglePacker(int packingAreaWidth, int packingAreaHeight)
		{
			private struct Point(int x, int y)
			{
				public int X = x;
				public int Y = y;
			}
			
			private class AnchorRankComparer : IComparer<Point>
			{
				public static readonly AnchorRankComparer Default = new();
				public int Compare(Point left, Point right) => (left.X + left.Y) - (right.X + right.Y);
			}

			private int _searchAreaHeight = 1;
			private int _searchAreaWidth = 1;
			
			private readonly List<Point> _anchors = [new(0, 0)];
			private readonly List<Rectangle> _packedRectangles = new();

			public bool TryPack(Rectangle rectangle, int padding)
			{
				var rectWidth = rectangle.Width + padding;
				var rectHeight = rectangle.Height + padding;
				var anchorIndex = SelectAnchorRecursive(rectWidth, rectHeight, _searchAreaWidth, _searchAreaHeight);

				if (anchorIndex == -1)
				{
					rectangle.X = rectangle.Y = 0;
					return false;
				}

				var placement = _anchors[anchorIndex];

				OptimizePlacement(placement, rectWidth, rectHeight);

				var blocksAnchor =
					((placement.X + rectWidth) > _anchors[anchorIndex].X) &&
					((placement.Y + rectHeight) > _anchors[anchorIndex].Y);

				if (blocksAnchor)
				{
					_anchors.RemoveAt(anchorIndex);
				}

				InsertAnchor(new Point(placement.X + rectWidth, placement.Y));
				InsertAnchor(new Point(placement.X, placement.Y + rectHeight));

				_packedRectangles.Add(new Rectangle(placement.X, placement.Y, rectWidth, rectHeight));
				rectangle.X = placement.X;
				rectangle.Y = placement.Y;

				return true;
			}
			
			private void OptimizePlacement(Point placement, int rectangleWidth, int rectangleHeight)
			{
				var rectangle = new Rectangle(placement.X, placement.Y, rectangleWidth, rectangleHeight);

				var leftMost = placement.X;
				while (IsFree(rectangle, packingAreaWidth, packingAreaHeight))
				{
					leftMost = rectangle.X;
					--rectangle.X;
				}

				rectangle.X = placement.X;

				var topMost = placement.Y;
				while (IsFree(rectangle, packingAreaWidth, packingAreaHeight))
				{
					topMost = rectangle.Y;
					--rectangle.Y;
				}

				if ((placement.X - leftMost) > (placement.Y - topMost))
				{
					placement.X = leftMost;
				}
				else
				{
					placement.Y = topMost;
				}
			}

			private int SelectAnchorRecursive(int rectWidth, int rectHeight, int testedAreaWidth, int testedAreaHeight)
			{
				while (true)
				{
					var freeAnchorIndex = FindFirstFreeAnchor(rectWidth, rectHeight, testedAreaWidth, testedAreaHeight);
					if (freeAnchorIndex >= 0)
					{
						_searchAreaWidth = testedAreaWidth;
						_searchAreaHeight = testedAreaHeight;

						return freeAnchorIndex;
					}

					var canEnlargeWidth = (testedAreaWidth < packingAreaWidth);
					var canEnlargeHeight = (testedAreaHeight < packingAreaHeight);
					var shouldEnlargeHeight = (!canEnlargeWidth) || (testedAreaHeight < testedAreaWidth);

					if (canEnlargeHeight && shouldEnlargeHeight)
					{
						testedAreaHeight = Math.Min(testedAreaHeight * 2, packingAreaHeight);
						continue;
					}

					if (canEnlargeWidth)
					{
						testedAreaWidth = Math.Min(testedAreaWidth * 2, packingAreaWidth);
						continue;
					}

					return -1;
				}
			}

			private int FindFirstFreeAnchor(int rectWidth, int rectHeight, int testedAreaWidth, int testedAreaHeight)
			{
				var potentialLocation = new Rectangle(0, 0, rectWidth, rectHeight);
				
				for (var index = 0; index < _anchors.Count; ++index)
				{
					potentialLocation.X = _anchors[index].X;
					potentialLocation.Y = _anchors[index].Y;

					if (IsFree(potentialLocation, testedAreaWidth, testedAreaHeight))
					{
						return index;
					}
				}

				return -1;
			}
			
			private bool IsFree(Rectangle rectangle, int testedPackingAreaWidth, int testedPackingAreaHeight)
			{
				var leavesPackingArea = 
					(rectangle.X < 0) || (rectangle.Y < 0) || 
					(rectangle.X + rectangle.Width > testedPackingAreaWidth) || 
					(rectangle.Y + rectangle.Height > testedPackingAreaHeight);

				return !leavesPackingArea && !IntersectsPackedRectangle(rectangle);
			}

			private bool IntersectsPackedRectangle(Rectangle rectangle)
			{
				return _packedRectangles.Any(packed =>
					rectangle.X < packed.X + packed.Width &&
					rectangle.X + rectangle.Width > packed.X &&
					rectangle.Y < packed.Y + packed.Height &&
					rectangle.Y + rectangle.Height > packed.Y);
			}
			
			private void InsertAnchor(Point anchor)
			{
				int insertIndex = _anchors.BinarySearch(anchor, AnchorRankComparer.Default);
				if (insertIndex < 0)
					insertIndex = ~insertIndex;

				_anchors.Insert(insertIndex, anchor);
			}
		}
		
		#endregion
	}
}
