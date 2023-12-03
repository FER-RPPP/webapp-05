﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RPPP_WebApp.Models
{
    public partial class RPPP05Context : DbContext
    {
        //public RPPP05Context()
        //{
        //}

        public RPPP05Context(DbContextOptions<RPPP05Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Dokument> Dokument { get; set; }
        public virtual DbSet<Ima> Ima { get; set; }
        public virtual DbSet<Kvalifikacija> Kvalifikacija { get; set; }
        public virtual DbSet<Partner> Partner { get; set; }
        public virtual DbSet<Posao> Posao { get; set; }
        public virtual DbSet<Projekt> Projekt { get; set; }
        public virtual DbSet<ProjektnaKartica> ProjektnaKartica { get; set; }
        public virtual DbSet<StatusZadatka> StatusZadatka { get; set; }
        public virtual DbSet<Suradnik> Suradnik { get; set; }
        public virtual DbSet<TipPartnera> TipPartnera { get; set; }
        public virtual DbSet<TipProjekta> TipProjekta { get; set; }
        public virtual DbSet<Transakcija> Transakcija { get; set; }
        public virtual DbSet<Uloga> Uloga { get; set; }
        public virtual DbSet<VrstaDokumenta> VrstaDokumenta { get; set; }
        public virtual DbSet<VrstaPosla> VrstaPosla { get; set; }
        public virtual DbSet<VrstaTransakcije> VrstaTransakcije { get; set; }
        public virtual DbSet<VrstaZahtjeva> VrstaZahtjeva { get; set; }
        public virtual DbSet<Zadatak> Zadatak { get; set; }
        public virtual DbSet<Zahtjev> Zahtjev { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dokument>(entity =>
            {
                entity.HasKey(e => e.IdDokument)
                    .HasName("PK__Dokument__420542B52EAF90A1");

                entity.Property(e => e.IdProjekt).HasColumnName("idProjekt");

                entity.Property(e => e.IdVrstaDok).HasColumnName("idVrstaDok");

                entity.Property(e => e.NazivDatoteka)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipDokument)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.VelicinaDokument)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdProjektNavigation)
                    .WithMany(p => p.Dokument)
                    .HasForeignKey(d => d.IdProjekt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Dokument__idProj__5F492382");

                entity.HasOne(d => d.IdVrstaDokNavigation)
                    .WithMany(p => p.Dokument)
                    .HasForeignKey(d => d.IdVrstaDok)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Dokument__idVrst__603D47BB");
            });

            modelBuilder.Entity<Ima>(entity =>
            {
                entity.HasKey(e => new { e.Oib, e.IdUloga })
                    .HasName("PK__Ima__53D3E463D4BDAF44");

                entity.Property(e => e.Oib).HasColumnName("OIB");

                entity.Property(e => e.IdUloga).HasColumnName("idUloga");

                entity.HasOne(d => d.IdUlogaNavigation)
                    .WithMany(p => p.Ima)
                    .HasForeignKey(d => d.IdUloga)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Ima__idUloga__725BF7F6");
            });

            modelBuilder.Entity<Kvalifikacija>(entity =>
            {
                entity.HasKey(e => e.IdKvalifikacija)
                    .HasName("PK__kvalifik__18FC9B98B5D0F7A1");

                entity.ToTable("kvalifikacija");

                entity.Property(e => e.IdKvalifikacija).HasColumnName("idKvalifikacija");

                entity.Property(e => e.NazivKvalifikacija)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivKvalifikacija");
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.HasKey(e => e.IdPartner)
                    .HasName("PK__Partner__4D67B84FDB563146");

                entity.HasIndex(e => e.EmailPartner, "UQ__Partner__3F193BFD7FBE925E")
                    .IsUnique();

                entity.HasIndex(e => e.Ibanpartner, "UQ__Partner__695C0D148BE013DB")
                    .IsUnique();

                entity.HasIndex(e => e.Oib, "UQ__Partner__CB394B3E6582DAB1")
                    .IsUnique();

                entity.Property(e => e.IdPartner).HasColumnName("idPartner");

                entity.Property(e => e.AdresaPartner)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("adresaPartner");

                entity.Property(e => e.EmailPartner)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("emailPartner");

                entity.Property(e => e.Ibanpartner)
                    .IsRequired()
                    .HasMaxLength(21)
                    .IsUnicode(false)
                    .HasColumnName("IBANPartner")
                    .IsFixedLength();

                entity.Property(e => e.IdTipPartnera).HasColumnName("idTipPartnera");

                entity.Property(e => e.NazivPartner)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nazivPartner");

                entity.Property(e => e.Oib)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("OIB")
                    .IsFixedLength();

                entity.HasOne(d => d.IdTipPartneraNavigation)
                    .WithMany(p => p.Partner)
                    .HasForeignKey(d => d.IdTipPartnera)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Partner__idTipPa__4E1E9780");
            });

            modelBuilder.Entity<Posao>(entity =>
            {
                entity.HasKey(e => e.IdPosao)
                    .HasName("PK__Posao__C7661B95CA43F656");

                entity.HasOne(d => d.IdVrstaPosaoNavigation)
                    .WithMany(p => p.Posao)
                    .HasForeignKey(d => d.IdVrstaPosao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Posao__IdVrstaPo__3DE82FB7");
            });

            modelBuilder.Entity<Projekt>(entity =>
            {
                entity.HasKey(e => e.IdProjekt)
                    .HasName("PK__Projekt__8FCCB4566ED920F8");

                entity.Property(e => e.IdProjekt).HasColumnName("idProjekt");

                entity.Property(e => e.IdTip).HasColumnName("idTip");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("naziv");

                entity.Property(e => e.Opis)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("opis");

                entity.Property(e => e.VrKraj)
                    .HasColumnType("date")
                    .HasColumnName("vrKraj");

                entity.Property(e => e.VrPocetak)
                    .HasColumnType("date")
                    .HasColumnName("vrPocetak");

                entity.HasOne(d => d.IdTipNavigation)
                    .WithMany(p => p.Projekt)
                    .HasForeignKey(d => d.IdTip)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Projekt__idTip__44952D46");

                entity.HasMany(d => d.IdPartner)
                    .WithMany(p => p.IdProjekt)
                    .UsingEntity<Dictionary<string, object>>(
                        "Narucio",
                        l => l.HasOne<Partner>().WithMany().HasForeignKey("IdPartner").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Narucio__idPartn__5C6CB6D7"),
                        r => r.HasOne<Projekt>().WithMany().HasForeignKey("IdProjekt").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Narucio__idProje__5B78929E"),
                        j =>
                        {
                            j.HasKey("IdProjekt", "IdPartner").HasName("PK__Narucio__6B1ACFD225EE3B16");

                            j.ToTable("Narucio");

                            j.IndexerProperty<int>("IdProjekt").HasColumnName("idProjekt");

                            j.IndexerProperty<int>("IdPartner").HasColumnName("idPartner");
                        });
            });

            modelBuilder.Entity<ProjektnaKartica>(entity =>
            {
                entity.HasKey(e => e.SubjektIban)
                    .HasName("PK__Projektn__871E209838A1B9E4");

                entity.ToTable("Projektna_kartica");

                entity.Property(e => e.SubjektIban)
                    .HasMaxLength(21)
                    .IsUnicode(false)
                    .HasColumnName("subjektIBAN")
                    .IsFixedLength();

                entity.Property(e => e.IdProjekt).HasColumnName("idProjekt");

                entity.Property(e => e.Valuta)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("valuta");

                entity.Property(e => e.VrijemeOtvaranja)
                    .HasColumnType("date")
                    .HasColumnName("vrijemeOtvaranja");

                entity.HasOne(d => d.IdProjektNavigation)
                    .WithMany(p => p.ProjektnaKartica)
                    .HasForeignKey(d => d.IdProjekt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Projektna__idPro__50FB042B");
            });

            modelBuilder.Entity<StatusZadatka>(entity =>
            {
                entity.HasKey(e => e.IdStatus)
                    .HasName("PK__StatusZa__01936F74A8CD31F4");

                entity.Property(e => e.IdStatus).HasColumnName("idStatus");

                entity.Property(e => e.NazivStatus)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivStatus");
            });

            modelBuilder.Entity<Suradnik>(entity =>
            {
                entity.HasKey(e => e.Oib);

                entity.HasIndex(e => e.Mail, "UQ__Suradnik__7A212904543F6C26")
                    .IsUnique();

                entity.HasIndex(e => e.Mobitel, "UQ__Suradnik__7E79A46778DC6E9B")
                    .IsUnique();

                entity.HasIndex(e => e.IdSuradnik, "UQ__Suradnik__B9075C9BDCFCBD79")
                    .IsUnique();

                entity.Property(e => e.Oib)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("OIB")
                    .IsFixedLength();

                entity.Property(e => e.IdKvalifikacija).HasColumnName("idKvalifikacija");

                entity.Property(e => e.IdPartner).HasColumnName("idPartner");

                entity.Property(e => e.IdSuradnik)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("idSuradnik");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ime");

                entity.Property(e => e.Mail)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("mail");

                entity.Property(e => e.Mobitel)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("mobitel");

                entity.Property(e => e.Prezime)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("prezime");

                entity.Property(e => e.Stranka)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("stranka");

                entity.HasOne(d => d.IdKvalifikacijaNavigation)
                    .WithMany(p => p.Suradnik)
                    .HasForeignKey(d => d.IdKvalifikacija)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Suradnik__idKval__65F62111");

                entity.HasOne(d => d.IdPartnerNavigation)
                    .WithMany(p => p.Suradnik)
                    .HasForeignKey(d => d.IdPartner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Suradnik__idPart__66EA454A");

                entity.HasMany(d => d.IdPosao)
                    .WithMany(p => p.Oib)
                    .UsingEntity<Dictionary<string, object>>(
                        "Radi",
                        l => l.HasOne<Posao>().WithMany().HasForeignKey("IdPosao").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Radi_Posao"),
                        r => r.HasOne<Suradnik>().WithMany().HasForeignKey("Oib").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Radi_Suradnik"),
                        j =>
                        {
                            j.HasKey("Oib", "IdPosao");

                            j.ToTable("Radi");

                            j.IndexerProperty<string>("Oib").HasMaxLength(11).IsUnicode(false).HasColumnName("OIB").IsFixedLength();
                        });
            });

            modelBuilder.Entity<TipPartnera>(entity =>
            {
                entity.HasKey(e => e.IdTipPartnera)
                    .HasName("PK__TipPartn__01BFB86166F4FE38");

                entity.Property(e => e.IdTipPartnera).HasColumnName("idTipPartnera");

                entity.Property(e => e.TipPartnera1)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tipPartnera");
            });

            modelBuilder.Entity<TipProjekta>(entity =>
            {
                entity.HasKey(e => e.IdTip)
                    .HasName("PK__TipProje__020FACBFD230405F");

                entity.Property(e => e.IdTip).HasColumnName("idTip");

                entity.Property(e => e.NazivTip)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivTip");
            });

            modelBuilder.Entity<Transakcija>(entity =>
            {
                entity.HasKey(e => e.PrimateljIban)
                    .HasName("PK__Transakc__64FB3C8E2F4EEF36");

                entity.Property(e => e.PrimateljIban)
                    .HasMaxLength(21)
                    .IsUnicode(false)
                    .HasColumnName("primateljIBAN")
                    .IsFixedLength();

                entity.Property(e => e.IdTransakcije)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("idTransakcije");

                entity.Property(e => e.Opis)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("opis");

                entity.Property(e => e.SubjektIban)
                    .IsRequired()
                    .HasMaxLength(21)
                    .IsUnicode(false)
                    .HasColumnName("subjektIBAN")
                    .IsFixedLength();

                entity.Property(e => e.Vrsta)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("vrsta");

                entity.HasOne(d => d.IdTransakcijeNavigation)
                    .WithMany(p => p.Transakcija)
                    .HasForeignKey(d => d.IdTransakcije)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transakci__idTra__6ABAD62E");

                entity.HasOne(d => d.SubjektIbanNavigation)
                    .WithMany(p => p.Transakcija)
                    .HasForeignKey(d => d.SubjektIban)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transakci__subje__69C6B1F5");
            });

            modelBuilder.Entity<Uloga>(entity =>
            {
                entity.HasKey(e => e.IdUloga)
                    .HasName("PK__uloga__8EAAF5C8ED59755D");

                entity.ToTable("uloga");

                entity.Property(e => e.IdUloga).HasColumnName("idUloga");

                entity.Property(e => e.IdProjekt).HasColumnName("idProjekt");

                entity.Property(e => e.NazivUloge)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivUloge");

                entity.HasOne(d => d.IdProjektNavigation)
                    .WithMany(p => p.Uloga)
                    .HasForeignKey(d => d.IdProjekt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__uloga__idProjekt__589C25F3");
            });

            modelBuilder.Entity<VrstaDokumenta>(entity =>
            {
                entity.HasKey(e => e.IdVrstaDok)
                    .HasName("PK__VrstaDok__D328B413502FC7CC");

                entity.Property(e => e.IdVrstaDok).HasColumnName("idVrstaDok");

                entity.Property(e => e.NazivVrstaDok)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivVrstaDok");
            });

            modelBuilder.Entity<VrstaPosla>(entity =>
            {
                entity.HasKey(e => e.IdVrstaPosao)
                    .HasName("PK__VrstaPos__BFA760BCC0389F06");

                entity.Property(e => e.NazivPosao)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivPosao");
            });

            modelBuilder.Entity<VrstaTransakcije>(entity =>
            {
                entity.HasKey(e => e.IdTransakcije)
                    .HasName("PK__VrstaTra__33A5BCE2EA084E63");

                entity.Property(e => e.IdTransakcije).HasColumnName("idTransakcije");

                entity.Property(e => e.NazivTransakcije)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivTransakcije");
            });

            modelBuilder.Entity<VrstaZahtjeva>(entity =>
            {
                entity.HasKey(e => e.IdVrsta)
                    .HasName("PK__VrstaZah__306017AD96E18C88");

                entity.Property(e => e.IdVrsta).HasColumnName("idVrsta");

                entity.Property(e => e.NazivVrsta)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nazivVrsta");
            });

            modelBuilder.Entity<Zadatak>(entity =>
            {
                entity.HasKey(e => e.IdZadatak)
                    .HasName("PK__Zadatak__2FCCCF8E17604AA5");

                entity.HasIndex(e => e.IdZadatak, "UQ__Zadatak__05C20C63C613B77D")
                    .IsUnique();

                entity.Property(e => e.IdZadatak).HasColumnName("idZadatak");

                entity.Property(e => e.IdStatus).HasColumnName("idStatus");

                entity.Property(e => e.IdZahtjev).HasColumnName("idZahtjev");

                entity.Property(e => e.Oibnositelj)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("OIBNositelj");

                entity.Property(e => e.VrKraj)
                    .HasColumnType("date")
                    .HasColumnName("vrKraj");

                entity.Property(e => e.VrKrajOcekivano)
                    .HasColumnType("date")
                    .HasColumnName("vrKrajOcekivano");

                entity.Property(e => e.VrPoc)
                    .HasColumnType("date")
                    .HasColumnName("vrPoc");

                entity.Property(e => e.Vrsta)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("vrsta");

                entity.HasOne(d => d.IdStatusNavigation)
                    .WithMany(p => p.Zadatak)
                    .HasForeignKey(d => d.IdStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Zadatak__idStatu__55BFB948");

                entity.HasOne(d => d.IdZahtjevNavigation)
                    .WithMany(p => p.Zadatak)
                    .HasForeignKey(d => d.IdZahtjev)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Zadatak__idZahtj__54CB950F");
            });

            modelBuilder.Entity<Zahtjev>(entity =>
            {
                entity.HasKey(e => e.IdZahtjev)
                    .HasName("PK__Zahtjev__DD398F69DAB9BA74");

                entity.Property(e => e.IdZahtjev).HasColumnName("idZahtjev");

                entity.Property(e => e.IdProjekt).HasColumnName("idProjekt");

                entity.Property(e => e.IdVrsta).HasColumnName("idVrsta");

                entity.Property(e => e.Opis)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("opis");

                entity.Property(e => e.Prioritet)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("prioritet");

                entity.Property(e => e.VrKraj)
                    .HasColumnType("date")
                    .HasColumnName("vrKraj");

                entity.Property(e => e.VrKrajOcekivano)
                    .HasColumnType("date")
                    .HasColumnName("vrKrajOcekivano");

                entity.Property(e => e.VrPocetak)
                    .HasColumnType("date")
                    .HasColumnName("vrPocetak");

                entity.HasOne(d => d.IdProjektNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(d => d.IdProjekt)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Zahtjev__idProje__4865BE2A");

                entity.HasOne(d => d.IdVrstaNavigation)
                    .WithMany(p => p.Zahtjev)
                    .HasForeignKey(d => d.IdVrsta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Zahtjev__idVrsta__477199F1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}