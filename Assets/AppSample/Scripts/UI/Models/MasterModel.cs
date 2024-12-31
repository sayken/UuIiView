using System.Collections.Generic;
using System.Collections;
using UuIiView;
using System.Linq;

namespace UuIiView.Sample
{
    public interface IMaster{
        string Id { get; }
    }

    public class MasterModel : IModel
    {
        Dictionary<System.Type, IList> masterDic = new Dictionary<System.Type, IList>();

        public MasterModel()
        {
        }

        public void Init()
        {
            var itemJson = UnityEngine.Resources.Load("Json/Master/ItemMaster") as UnityEngine.TextAsset;
            var itemMaster = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemMaster>>(itemJson.text);
            Add(itemMaster);
        }

        public void Add<T>(List<T> data) where T : IMaster
        {
            masterDic.Add(typeof (T), data);
        }

        public T FindById<T>(string id) where T : IMaster
        {
            if ( masterDic.TryGetValue(typeof(T), out var ilist) )
            {
                var mstList = (List<T>)ilist;
                IMaster mst = mstList.FirstOrDefault(x=>x.Id == id);
                return (T)mst;
            }
            return default(T);
        }

        public List<T> GetAll<T>() where T : IMaster
        {
            if ( masterDic.TryGetValue(typeof(T), out var ilist) )
            {
                return (List<T>)ilist;
            }
            return null;
        }
    }
}