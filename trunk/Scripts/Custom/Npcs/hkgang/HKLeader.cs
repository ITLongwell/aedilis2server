using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.HunterKiller
{
	public class HKLeader : HKMobile
	{
		[Constructable]
		public HKLeader() : base( AIType.AI_Melee, FightMode.Closest )
		{
			SetStr( 200, 300 );
			SetDex( 60, 100 );
			SetInt( 60, 100 );

			SetHits( 151, 210 );

			SetDamage( 7, 9 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.Anatomy, 80.1, 100.0 );
			SetSkill( SkillName.Fencing, 80.1, 100.0 );
			SetSkill( SkillName.Macing, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 120.0, 130.0 );
            SetSkill( SkillName.Healing, 65.0, 100.0 ); 
			SetSkill( SkillName.Swords, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 22;

			new Horse().Rider = this;

			PackWeapon(2,4);
		}
	
		public HKLeader( Serial serial ) : base( serial )
		{
		}	

		public void Speak(int s)
		{
			string[] toSay = new string[]
			{
				"Common guys ! Lets spill some blood !",
				"We must wait a little for his return...",
				"Enough ! Heading home !"
			};

			Say( true, toSay[s] );	
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