using Microsoft.AspNetCore.Mvc;

using web_Aqua.Context;
using web_Aqua.Models;

namespace web_Aqua.Controllers
{
    public class ContactAndPolicyController : Controller
    {

        db_aquaponicsContext db_Context = new db_aquaponicsContext();
        public IActionResult Policy(int? ID)
        {
            HomeModel model = new HomeModel();

            model.listCAP = db_Context.ContactAndPolicies.Where(p => p.KeyWord == "Policy").ToList();
            var id = model.listCAP.FirstOrDefault();

            if(ID != null)
                model.contactAndPolicy = db_Context.ContactAndPolicies.Where(p => p.KeyWord == "Policy").Where(p=>p.Id==ID).FirstOrDefault();
            else
                model.contactAndPolicy = db_Context.ContactAndPolicies.Where(p => p.KeyWord == "Policy").Where(p => p.Id == id.Id).FirstOrDefault();


            return View(model);
        }


        public IActionResult Contact()
        {
            var contact = db_Context.ContactAndPolicies.Where(c => c.KeyWord == "Contact").FirstOrDefault();
            return View(contact);
        }


    }
}
