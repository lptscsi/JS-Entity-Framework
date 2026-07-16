namespace RIAPP.DataService.Core.Types
{

    public class Permissions
    {
        public Permissions()
        {

        }

        public PermissionList Items { get; set; } = new PermissionList();
    }
}