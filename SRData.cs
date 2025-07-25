using System.Reflection;
using Blazored.LocalStorage;

namespace Mas.Infrastructure.BlazorComponents
{
    public class StoredSrData : IComparable<StoredSrData>
    {
        public ulong InterfaceId { get; set; } = 0;
        public string SturdyRef { get; set; } = "";
        public string PetName { get; set; } = "";
        public bool AutoConnect { get; set; } = false;
        public bool DefaultSelect { get; set; } = false;

        public int CompareTo(StoredSrData? other)
        {
            // A null value means that this object is greater.
            if (other == null)
                return 1;
            if (PetName != "" && other.PetName != "")
                return string.Compare(PetName, other.PetName, StringComparison.Ordinal);
            else
                return string.Compare(SturdyRef, other.SturdyRef, StringComparison.Ordinal);
        }

        public static string StorageKey { get; set; } = "sturdy-ref-store";

        public static async Task<List<StoredSrData>> GetAllData(ILocalStorageService service)
        {
            return await service.GetItemAsync<List<StoredSrData>>(StorageKey) ?? [];
        }

        public static async Task<List<StoredSrData>> SaveNew(ILocalStorageService service, StoredSrData newData)
        {
            var all = await GetAllData(service);
            all.Add(newData);
            return await SaveAllData(service, all);
        }

        public static async Task<List<StoredSrData>> SaveAllData(ILocalStorageService service, List<StoredSrData> allData)
        {
            await service.SetItemAsync(StorageKey, allData);
            return allData;
        }
    }

    public static class ExtensionMethods
    {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            if (@this.Invoke(obj, parameters) is not Task task) return Task.CompletedTask;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            if (resultProperty == null) return Task.CompletedTask;
            var res = resultProperty.GetValue(task);
            return res ?? Task.CompletedTask;
        }
    }
}
