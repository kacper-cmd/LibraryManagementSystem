using Client.Config;
using Microsoft.AspNetCore.Components;

namespace Client.Components.Toast
{
	public partial class BlazoredToast
	{
		[CascadingParameter] private BlazoredToasts ToastsContainer {  get; set; }
		[Parameter] public Guid ToastId { get; set; }
		[Parameter] public ToastSettings ToastSettings { get; set; }
		private void Close()
		{
			ToastsContainer.RemoveToast(ToastId);
		}
	}
}
