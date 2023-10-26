using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Application_Recorder.Services
{
	public class DialogService : AvaloniaObject
	{
		private static readonly Dictionary<object, Visual> RegistrationMapper =
			new Dictionary<object, Visual>();

		static DialogService()
		{
			RegisterProperty.Changed.Subscribe(RegisterChanged);
		}

		private static void RegisterChanged(AvaloniaPropertyChangedEventArgs<object?> e)
		{
			if (e.Sender is not Visual sender)
			{
				throw new InvalidOperationException("The DialogService can only be registered on a Visual");
			}

			// Unregister any old registered context
			if (e.OldValue.Value != null)
			{
				RegistrationMapper.Remove(e.OldValue.Value);
			}

			// Register any new context
			if (e.NewValue.Value != null)
			{
				RegistrationMapper.Add(e.NewValue.Value, sender);
			}
		}

		/// <summary>
		/// This property handles the registration of Views and ViewModel
		/// </summary>
		public static readonly AttachedProperty<object?> RegisterProperty = AvaloniaProperty.RegisterAttached<DialogService, Visual, object?>(
			"Register");

		/// <summary>
		/// Accessor for Attached property <see cref="RegisterProperty"/>.
		/// </summary>
		public static void SetRegister(AvaloniaObject element, object value)
		{
			element.SetValue(RegisterProperty, value);
		}

		/// <summary>
		/// Accessor for Attached property <see cref="RegisterProperty"/>.
		/// </summary>
		public static object? GetRegister(AvaloniaObject element)
		{
			return element.GetValue(RegisterProperty);
		}

		/// <summary>
		/// Gets the associated <see cref="Visual"/> for a given context. Returns null, if none was registered
		/// </summary>
		/// <param name="context">The context to lookup</param>
		/// <returns>The registered Visual for the context or null if none was found</returns>
		public static Visual? GetVisualForContext(object context)
		{
			return RegistrationMapper.TryGetValue(context, out var result) ? result : null;
		}

		/// <summary>
		/// Gets the parent <see cref="TopLevel"/> for the given context. Returns null, if no TopLevel was found
		/// </summary>
		/// <param name="context">The context to lookup</param>
		/// <returns>The registered TopLevel for the context or null if none was found</returns>
		public static TopLevel? GetTopLevelForContext(object context)
		{
			return TopLevel.GetTopLevel(GetVisualForContext(context));
		}
	}

	/// <summary>
	/// A helper class to manage dialogs via extension methods. Add more on your own
	/// </summary>
	public static class DialogHelper
	{

		/// <summary>
		/// Shows an open file dialog for a registered context, most likely a ViewModel
		/// </summary>
		/// <param name="context">Context of the given dialog.</param>
		/// <param name="title">The dialog title or a default is null.</param>
		/// <param name="selectMany">If true, multiple files are selectable. 
		/// Otherwise, is false and only one file is selectable.</param>
		/// <returns>An array of file names.</returns>
		/// <exception cref="ArgumentNullException">if context was null</exception>
		public static async Task<IEnumerable<string>?> OpenFileDialogAsync
		(this object? context, string? title = null, bool selectMany = false)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			// lookup the TopLevel for the context
			var topLevel = DialogService.GetTopLevelForContext(context);

			if (topLevel != null)
			{
				// Open file dialog
				IReadOnlyList<IStorageFile>? storageFile =
				await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()

				{
					Title = title ?? "Select JSON file",
					AllowMultiple = selectMany,
					FileTypeFilter = new FilePickerFileType[]
				{
				new("JSON File")
				{
					Patterns = new[] {"*.json"},
					AppleUniformTypeIdentifiers = new[] {"public.json"},
					MimeTypes = new[] { "application/json" }
				}
				}
				});

				// return the result
				return storageFile.Select(s => s.Name);
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context">Context of the given dialog.</param>
		/// <param name="title">The dialog title or a default is null.</param>
		/// <param name="contents">Represents the contents of a saved file.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<IEnumerable<string>?> SaveFileDialogAsync
		(this object? context, string? title = null, string? contents = null)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			// lookup the TopLevel for the context
			var topLevel = DialogService.GetTopLevelForContext(context);

			if (topLevel != null)
			{
				IStorageFile? saveFile =
				await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
				{
					Title = title ?? "Save File as JSON",
					SuggestedFileName = "JobAppData",
					ShowOverwritePrompt = true,
					FileTypeChoices = new FilePickerFileType[]
					{
						new("JSON File")
						{
							Patterns = new[] {"*.json"},
							AppleUniformTypeIdentifiers = new[] {"public.json"},
							MimeTypes = new[] { "application/json" }
						}
					}
				});

			}
			return null;

		}

	}
}
