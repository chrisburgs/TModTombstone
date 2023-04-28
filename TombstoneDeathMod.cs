using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader;
using Terraria.UI;


namespace ChrisTrashstone
{
    public class TombstoneDeathMod: Terraria.ModLoader.Mod
    {
        public static ModKeybind DeathLocatorHotKey;
        public static DeathLocator deathLocatorUI;
        private UserInterface wheresMyItemsUserInterface;
		int lastSeenScreenWidth;
		int lastSeenScreenHeight;
        public override void Load()
        {   
            if (!Main.dedServ)
			{
                DeathLocatorHotKey = KeybindLoader.RegisterKeybind(this, "Show Search Interface", "P");
                deathLocatorUI = new DeathLocator();
                deathLocatorUI.Activate();
                wheresMyItemsUserInterface = new UserInterface();
                wheresMyItemsUserInterface.SetState(deathLocatorUI);
            }

        }

        public void Send(int toWho, int fromWho, int x, int y)
        {
            ModPacket packet = GetPacket();
            if (Main.netMode == NetmodeID.Server)
            {
                packet.Write(fromWho);
            }
            packet.Write(x);
            packet.Write(y);
            packet.Send(toWho, fromWho);
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                fromWho = reader.ReadInt32();
            }

            int x = reader.ReadInt32();
            int y = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                Send(-1, fromWho, x ,y );
                //NetworkText text = NetworkText.FromFormattable("Server removing gravestone for {0}", Main.player[fromWho].name);
                //NetMessage.BroadcastChatMessage(text, Color.White);
                WorldGen.KillTile(x, y);
            }
            else
            {
                //Main.NewText("Remote player removing tombstone for " + Main.player[fromWho].name, 255, 100, 100);
                WorldGen.KillTile(x, y);
            }
        }
        public void UpdateUI(GameTime gameTime) {
			if (DeathLocator.visible) 
            {
                Point playerPosition = Main.player[0].position.ToTileCoordinates();
			    deathLocatorUI.updateDeathLocator(playerPosition.X, playerPosition.Y);
            	if (wheresMyItemsUserInterface != null)
					wheresMyItemsUserInterface.Update(gameTime);
            }
		}
        public static string hoverItemNameBackup;
		public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int vanillaInventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
			if (vanillaInventoryLayerIndex != -1) {
				layers.Insert(vanillaInventoryLayerIndex, new LegacyGameInterfaceLayer(
					"WheresMyItems: Quick Search",
					delegate {
						hoverItemNameBackup = null;
						if (DeathLocator.visible) {
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight) {
								wheresMyItemsUserInterface.Recalculate();
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}
							wheresMyItemsUserInterface.Draw(Main.spriteBatch, new GameTime());
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
			int mouseTextLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextLayerIndex != -1) {
				layers.Insert(mouseTextLayerIndex, new LegacyGameInterfaceLayer(
					"WheresMyItems: Hover Text Logic",
					delegate {
						if (!string.IsNullOrEmpty(TombstoneDeathMod.hoverItemNameBackup))
							Main.hoverItemName = TombstoneDeathMod.hoverItemNameBackup;
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}


        public class TombstoneDeathModSystem : ModSystem {
            public override void UpdateUI(GameTime gameTime) => ModContent.GetInstance<TombstoneDeathMod>().UpdateUI(gameTime);

            public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) => ModContent.GetInstance<TombstoneDeathMod>().ModifyInterfaceLayers(layers);
        }

    }
}
