using System;
using System.Linq;
using System.Web.Mvc;
using Solita.RedirectManager.Models;
using Solita.RedirectManager.Models.Api;

namespace Solita.RedirectManager.Controllers.Api
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class RewriteMappingsController : Controller
    {
        private readonly IRewritemapService _rewritemapService;

        public RewriteMappingsController(IRewritemapService rewritemapService)
        {
            _rewritemapService = rewritemapService;
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var mappings = _rewritemapService.ListMappings().Select(s => s.ToDto());
            var model = new RewriteMappings(mappings);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var mapping = _rewritemapService.GetMapping(id)?.ToDto();
            
            return Json(mapping, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FindMapping(string host, string path)
        {
            var mapping = _rewritemapService.GetMapping(host, path)?.ToDto();
            return Json(mapping, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(RewritemappingDto mapping)
        {
            if (mapping.Id != 0)
            {
                throw new Exception("Mapping already has an id");
            }

            var model = mapping.ToModel();
            _rewritemapService.Save(model);
            var newDto = model.ToDto();

            return Json(newDto);
        }

        [HttpPut]
        public JsonResult Edit(RewritemappingDto mapping)
        {
            var model = mapping.ToModel();
            _rewritemapService.Save(model);
            var newDto = model.ToDto();

            return Json(newDto);
        }


        [HttpDelete]
        public JsonResult Delete(int id)
        {
            _rewritemapService.Delete(id);

            return Json("Ok");
        }
    }
}