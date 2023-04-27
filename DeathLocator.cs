using Microsoft.Xna.Framework;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using ReLogic.Content;
using Terraria.Audio;
using System;
using System.IO;
using System.Net;

namespace TombstoneDeathMod2
{
	public class DeathLocator : UIState
	{
		public static UIPanel searchBarPanel;
		public static bool visible = false;
		// public static NewUITextBox box;
		public static List<DrawData[]> worldZoomDrawDatas = new List<DrawData[]>();
		public static List<Item> worldZoomItems = new List<Item>();
		public static List<Vector2> worldZoomPositions = new List<Vector2>();

		public static string history = "";

		public override void OnInitialize()
		{
			searchBarPanel = new UIPanel();
			searchBarPanel.SetPadding(0);
			searchBarPanel.Top.Set(50, 0f);
			searchBarPanel.HAlign = 0.5f;
			Append(searchBarPanel);
		}

		private Vector2 offset;

		public static bool dragging = false;

		private void DragStart(UIMouseEvent evt, UIElement listeningElement)
		{
			offset = new Vector2(evt.MousePosition.X - searchBarPanel.Left.Pixels, evt.MousePosition.Y - searchBarPanel.Top.Pixels);
			dragging = true;
		}

		private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
		{
			Vector2 end = evt.MousePosition;
			dragging = false;

			searchBarPanel.Left.Set(end.X - offset.X, 0f);
			searchBarPanel.Top.Set(end.Y - offset.Y, 0f);

			Recalculate();
		}

		// private void TogHover(UIMouseEvent evt, UIElement listeningElement)
		// {
		// 	UIHoverImageButton button = (evt.Target as UIHoverImageButton);
		// 	WheresMyItemsPlayer.hover = !WheresMyItemsPlayer.hover;
		// 	button.hoverText = "Click to switch peek modes: Show " + (WheresMyItemsPlayer.hover ? "Hovered" : "All");
		// }

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (searchBarPanel.ContainsPoint(MousePosition))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
			if (dragging)
			{
				searchBarPanel.Left.Set(MousePosition.X - offset.X, 0f);
				searchBarPanel.Top.Set(MousePosition.Y - offset.Y, 0f);
				Recalculate();
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Main.spriteBatch.End();
			Terraria.GameInput.PlayerInput.SetZoom_World();
			Matrix transformMatrix = Main.GameViewMatrix.ZoomMatrix;
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transformMatrix);

			for (int i = 0; i < worldZoomDrawDatas.Count; i++)
			{
				worldZoomDrawDatas[i][0].Draw(spriteBatch);
				if (worldZoomItems[i] != null)
				{
					Item curItem = worldZoomItems[i];
					float oldScale = Main.inventoryScale;
					Main.inventoryScale = .8f;
					ItemSlot.Draw(spriteBatch, ref curItem, 21, worldZoomPositions[i]);
					Main.inventoryScale = oldScale;
				}
				else
				{
					worldZoomDrawDatas[i][1].Draw(spriteBatch);
				}
			}
		}

		public void updateDeathLocator(int x, int y) 
        {
            int elementNumber = 0;
            playerLocationPanel(x, y);
			Player player = Main.player[Main.myPlayer];
			TombstonePlayer tStonePlayer = player.GetModPlayer<TombstonePlayer>();

            foreach (var item in tStonePlayer.playerDeathInventoryMap) {
                addDeathPanel(elementNumber, item.Key.ToString(), item.Value.getValue().ToString());
                elementNumber++;
            }
        }
		public static void playerLocationPanel(int x, int y) {
			searchBarPanel.RemoveAllChildren();
			UITextPanel<string> textPanel = new UITextPanel<string>("X: " + x + " Y: " + y, 1f, false);
			textPanel.Height.Set(0, 1f);
			textPanel.Top.Set(0, 0);
			searchBarPanel.Append(textPanel);
		}
		public static void addDeathPanel(int elementNumber, String location, String value) {
			UITextPanel<string> textPanel = new UITextPanel<string>(location + " : " + value, 1f, false);
			textPanel.Height.Set(20f, 1f);
			textPanel.Top.Set((40 * (elementNumber + 1)), 0);
			searchBarPanel.Append(textPanel);
		}
	}
}