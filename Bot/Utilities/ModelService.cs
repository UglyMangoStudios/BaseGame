using Discord;
using Discord.WebSocket;

namespace SpaceDiscordBot.Utilities
{
	/// <summary>
	/// A class that allows for the easy use of calling a method when a modal gets submitted. 
	/// </summary>
	internal static class ModelService
	{
		// The collection of model ids to a function
		private static readonly Dictionary<string, Func<SocketModal, Task>> s_modalEventDict = new();

		/// <summary>
		/// Subscribed by <see cref="Program"/>. Gets called whenever a modal gets submitted.
		/// </summary>
		/// <param name="modalData"></param>
		/// <returns></returns>
		public static async Task OnModalSubmit(SocketModal modalData)
		{
			//TODO: Add a check to see if the modal exists in the dict. If not, then return
			//TODO: After a function has been invoked, remove it from memory. Easy spot for a memory leak.
			string id = modalData.Data.CustomId;
			ulong userId = modalData.User.Id;
			Func<SocketModal, Task> onModalSubmitCall = s_modalEventDict[id + userId];

			await onModalSubmitCall.Invoke(modalData);
		}

		/// <summary>
		/// Creates a new modal and subsribes that modal to whenever it gets submitted.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userId">The Id of the User who called for this modal's creation</param>
		/// <param name="onModalSubmitCall"></param>
		/// <returns></returns>
		public static ModalBuilder CreateModal(string id, ulong userId, Func<SocketModal, Task> onModalSubmitCall)
		{
			//TODO: Subscribe the modal id with the user id. Otherwise another users modal may be replaced by a new modal that was created
			var modal = new ModalBuilder().WithCustomId(id);
			s_modalEventDict[id + userId] = onModalSubmitCall;

			return modal;
		}

		/// <summary>
		/// Converts all of the modal details into a dictionary for easy access. 
		/// </summary>
		/// <param name="modal">The modal to dissect</param>
		/// <returns>The dictionary with key/value with a format as field-id/field-value </returns>
		public static Dictionary<string, string> ConvertModalToDict(this SocketModal modal)
		{
			Dictionary<string, string> dict = new();

			foreach(var comp in modal.Data.Components)
			{
				if (comp.Type != ComponentType.TextInput) continue;
				dict[comp.CustomId] = comp.Value;
			}

			return dict;
		}


	}
}
