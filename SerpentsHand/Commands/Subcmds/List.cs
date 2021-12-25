using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace SerpentsHand.Commands.Subcmds
{
	public class List : ICommand
	{
		public string Command { get; } = "list";
		public string[] Aliases { get; } = { "l" };
		public string Description { get; } = "Shows a list with players that are currently Serpents Hand.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission("sh.list"))
			{
				response = "You don't have permission to execute this command. Required permission: sh.list";
				return false;
			}

			response = "\nList of players that are currently Serpent's Hand:\n";

			foreach (Player sHPlayer in API.GetSHPlayers())
				response += $"- ({sHPlayer.Id}) {sHPlayer.Nickname}\n";

			return true;
		}
	}
}
