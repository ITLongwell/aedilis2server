using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class EasterCarrot : Food
	{
		[Constructable]
		public EasterCarrot() : base( 0xC78 )
		{
                  Name = "Easter Bunny's Carrot";
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public EasterCarrot( Serial serial ) : base( serial )
		{
		}
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060662, "Happy Easter\t2006" );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}