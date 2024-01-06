namespace RIAppDemo.DAL.EF
{
    public class CustomerContact
    {
        public CustomerContact()
        {
        }

        public string EmailAddress { get; set; }

        public string Phone { get; set; }
    }

    public class CustomerName
    {
        public CustomerName()
        {
            Contact = new CustomerContact();
        }

        public string FirstName { get; set; }


        public string MiddleName { get; set; }


        public string LastName { get; set; }

        public CustomerContact Contact { get; set; }
    }


}
