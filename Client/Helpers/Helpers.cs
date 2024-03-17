using Syncfusion.Blazor.Popups;

namespace Client.Helpers
{
    public static class Helpers
    {
        public static async Task<bool> DisplayMessage(bool flag, string message, SfDialogService dialogService)
        {
            if (flag)
            {
                await dialogService.AlertAsync(message, "Success Operation");
                return true;
            }
            else
            {
                await dialogService.AlertAsync(message, "Alert!");
                return false;
            }
        }
    }
}
