using System;

namespace TalkiPlay.Shared
{
    public interface IChild
    {
       int Id { get; }
       string Name { get; }
       
       string PhotoPath { get; }
       int? AssetId { get; }
       DateTime? DateOfBirth { get;}
       
       int Age { get; }

       ChildCommunicationLevel? CommunicationLevel { get; set; }

       ChildResponseLevel? ResponseLevel { get; set; }
       
       ChildLanguageLevel? LanguageLevel { get;set; }
       int? FavouritePackId { get; set; }
       
    }
}