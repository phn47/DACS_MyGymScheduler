using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace toanggg.Data;

public partial class LinhContext : DbContext
{
    public LinhContext()
    {
    }

    public LinhContext(DbContextOptions<LinhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassSchedule> ClassSchedules { get; set; }

    public virtual DbSet<Gym> Gyms { get; set; }

    public virtual DbSet<GymType> GymTypes { get; set; }

    public virtual DbSet<MembershipType> MembershipTypes { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClassRegistration> UserClassRegistrations { get; set; }

    public virtual DbSet<UserMembership> UserMemberships { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-T4RGGNJJ;Initial Catalog=DACS;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__FDF4798690837EC4");

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassType).HasColumnName("class_type");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.GymId).HasColumnName("gym_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TrainerId).HasColumnName("trainer_id");

            entity.HasOne(d => d.Gym).WithMany(p => p.Classes)
                .HasForeignKey(d => d.GymId)
                .HasConstraintName("FK__Classes__gym_id__38996AB5");

            entity.HasOne(d => d.Trainer).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK__Classes__trainer__398D8EEE");
        });

        modelBuilder.Entity<ClassSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__ClassSch__C46A8A6F0C972833");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.EmailconfirmationToken).HasColumnName("emailconfirmation_token");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.IsemailOnfirmed).HasColumnName("isemail_onfirmed");
            entity.Property(e => e.Room).HasColumnName("room");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Weekday).HasColumnName("weekday");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ClassSche__class__3A81B327");

            entity.HasOne(d => d.User).WithMany(p => p.ClassSchedules)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ClassSchedules_Users");
        });

        modelBuilder.Entity<Gym>(entity =>
        {
            entity.HasKey(e => e.GymId).HasName("PK__Gyms__3EC25F6957570AA2");

            entity.Property(e => e.GymId).HasColumnName("gym_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Amenities)
                .HasColumnType("text")
                .HasColumnName("amenities");
            entity.Property(e => e.CloseHours).HasColumnName("close_hours");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Images)
                .HasColumnType("text")
                .HasColumnName("images");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.OpenHours).HasColumnName("open_hours");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.PriceRange).HasColumnName("price_range");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Website).HasColumnName("website");
        });

        modelBuilder.Entity<GymType>(entity =>
        {
            entity.HasKey(e => e.GymTypeId).HasName("PK__GymTypes__4D579759A29D52CB");

            entity.Property(e => e.GymTypeId).HasColumnName("gym_type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasMany(d => d.Gyms).WithMany(p => p.GymTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "GymTypeGym",
                    r => r.HasOne<Gym>().WithMany()
                        .HasForeignKey("GymId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GymTypeGy__gym_i__3C69FB99"),
                    l => l.HasOne<GymType>().WithMany()
                        .HasForeignKey("GymTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GymTypeGy__gym_t__3D5E1FD2"),
                    j =>
                    {
                        j.HasKey("GymTypeId", "GymId").HasName("PK__GymTypeG__CEBBB2AFDBE9EAC9");
                        j.ToTable("GymTypeGyms");
                        j.IndexerProperty<int>("GymTypeId").HasColumnName("gym_type_id");
                        j.IndexerProperty<int>("GymId").HasColumnName("gym_id");
                    });
        });

        modelBuilder.Entity<MembershipType>(entity =>
        {
            entity.HasKey(e => e.MembershipTypeId).HasName("PK__Membersh__7F6211FBF3C330DE");

            entity.Property(e => e.MembershipTypeId).HasColumnName("membership_type_id");
            entity.Property(e => e.AccessHours).HasColumnName("access_hours");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GymTypeAccess).HasColumnName("gym_type_access");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.GymTypeAccessNavigation).WithMany(p => p.MembershipTypes)
                .HasForeignKey(d => d.GymTypeAccess)
                .HasConstraintName("FK__Membershi__gym_t__403A8C7D");

            entity.HasMany(d => d.Classes).WithMany(p => p.MembershipTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "MembershipTypeClass",
                    r => r.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Membershi__class__3E52440B"),
                    l => l.HasOne<MembershipType>().WithMany()
                        .HasForeignKey("MembershipTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Membershi__membe__3F466844"),
                    j =>
                    {
                        j.HasKey("MembershipTypeId", "ClassId").HasName("PK__Membersh__00BD566359D3748A");
                        j.ToTable("MembershipTypeClasses");
                        j.IndexerProperty<int>("MembershipTypeId").HasColumnName("membership_type_id");
                        j.IndexerProperty<int>("ClassId").HasColumnName("class_id");
                    });
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("PK__Trainers__65A4B62976FE99C7");

            entity.Property(e => e.TrainerId).HasColumnName("trainer_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Expertise).HasColumnName("expertise");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.GymId).HasColumnName("gym_id");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Gym).WithMany(p => p.Trainers)
                .HasForeignKey(d => d.GymId)
                .HasConstraintName("FK__Trainers__gym_id__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.Trainers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Trainers__user_i__4222D4EF");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F8DC407D1");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164C4B750C7").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Confirmemail).HasColumnName("confirmemail");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmailconfirmationToken).HasColumnName("emailconfirmation_token");
            entity.Property(e => e.FacebbookId).HasColumnName("facebbook_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsemailOnfirmed).HasColumnName("isemail_onfirmed");
            entity.Property(e => e.MembershipTypeId).HasColumnName("membership_type_id");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url ");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.MembershipType).WithMany(p => p.Users)
                .HasForeignKey(d => d.MembershipTypeId)
                .HasConstraintName("FK__Users__membershi__46E78A0C");
        });

        modelBuilder.Entity<UserClassRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__UserClas__22A298F62D53AD27");

            entity.Property(e => e.RegistrationId).HasColumnName("registration_id");
            entity.Property(e => e.ClassScheduleId).HasColumnName("class_schedule_id");
            entity.Property(e => e.RegistrationDate).HasColumnName("registration_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ClassSchedule).WithMany(p => p.UserClassRegistrations)
                .HasForeignKey(d => d.ClassScheduleId)
                .HasConstraintName("FK__UserClass__class__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.UserClassRegistrations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserClass__user___440B1D61");
        });

        modelBuilder.Entity<UserMembership>(entity =>
        {
            entity.HasKey(e => e.UserMembershipId).HasName("PK__UserMemb__E37A2534036E95E7");

            entity.Property(e => e.UserMembershipId).HasColumnName("user_membership_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.MembershipTypeId).HasColumnName("membership_type_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.MembershipType).WithMany(p => p.UserMemberships)
                .HasForeignKey(d => d.MembershipTypeId)
                .HasConstraintName("FK__UserMembe__membe__44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.UserMemberships)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserMembe__user___45F365D3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
