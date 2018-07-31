using System.Collections.Generic;

namespace TMDT.Web.Models.DataContracts
{
    public class SavePermissionRequest
    {
        public string FunctionId { set; get; }

        public IList<PermissionViewModel> Permissions { get; set; }
    }
}