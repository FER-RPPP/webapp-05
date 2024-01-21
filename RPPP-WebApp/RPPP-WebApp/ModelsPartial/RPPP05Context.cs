
using Microsoft.EntityFrameworkCore;


namespace RPPP_WebApp.Models
{
    public partial class RPPP05Context
    {
        //public virtual DbSet<ViewPartner> vw_Partner { get; set; }
        public virtual DbSet<ViewDokumentInfo> vw_Dokumenti { get; set; }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ViewDokumentInfo>(entity => {
                entity.HasNoKey();
                //entity.ToView("vw_Dokumenti"); //u slučaju da se DbSet svojstvo zove drugačije
            });

        }
    }
}
