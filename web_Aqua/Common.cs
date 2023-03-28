using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.Data;
using System.Drawing;
using System.Reflection;
using web_Aqua.Context;
using web_Aqua.Models;



namespace web_Aqua
{
	public class Common
	{


		[NonAction]
		public SelectList ToSelectList(DataTable table, string valueField, string textField)
		{

			List<SelectListItem> list = new List<SelectListItem>();
			foreach (DataRow row in table.Rows)
			{
				list.Add(new SelectListItem()
				{
					Text = row[textField].ToString(),
					Value = row[valueField].ToString()
				});
			}
			return new SelectList(list, "Value", "Text");
		}

		public class ListtoDataTableConverter
		{
			public DataTable ToDataTable<T>(List<T> items)
			{
				DataTable dataTable = new DataTable(typeof(T).Name);
				PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo prop in Props)
				{
					dataTable.Columns.Add(prop.Name);
				}
				foreach (T item in items)
				{
					var values = new object[Props.Length];
					for (int i = 0; i < Props.Length; i++)
					{
						values[i] = Props[i].GetValue(item, null);
					}
					dataTable.Rows.Add(values);
				}
				return dataTable;
			}
		}

        public class Show
        {
            public string Name { get; set; }
            public bool ID { get; set; }
        }
    }


}
