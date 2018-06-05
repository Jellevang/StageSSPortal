using System.Linq;
using System.Data.Entity.Validation;

namespace DAL.EF
{
    public partial class StageSSPortalDbContext
    {
        //Een overide van de SaveChanges methode zodat deze exceptions kan opvangen indien er iets misloopt.
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);
                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                // Throw a new DbEntityValidationException with the improved exception message.
                // throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                return 1;
            }
        }
    }
}
