using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunication.Classes.Database.Objects
{
    public class RachunkiDbObject : DbObject<RachunkiDbObject>
    {
        [DbField(Type=FieldType.PrimaryKey,Constraint = FieldConstraint.NotNull)]
        public int IdRachunku { get => Get<int>(); set => Set(value); }

        [DbField(Constraint = FieldConstraint.NotNull)]
        public string Wpisujacy { get => Get<string>(); set => Set(value); }

        [DbField(Constraint = FieldConstraint.NotNull)]
        public float Kwota { get => Get<float>(); set => Set(value); }

        public RachunkiDbObject()
        {
            TableName = "Rachunki_T";
        }

        public override bool InitializeFromObject(RachunkiDbObject Object)
        {
            this.IdRachunku = Object.IdRachunku;
            this.Wpisujacy = Object.Wpisujacy;
            this.Kwota = Object.Kwota;

            return true;
        }
    }
}
