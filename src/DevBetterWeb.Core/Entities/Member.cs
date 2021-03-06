﻿using System;
using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities
{
  public class Member : BaseEntity
  {
    public Member()
    {
      UserId = "";
    }

    /// <summary>
    /// Members should only be created via the IMemberRegistrationService.
    /// This will fire off a NewMemberCreatedEvent
    /// </summary>
    /// <param name="userId"></param>
    internal Member(string userId)
    {
      UserId = userId;
      Events.Add(new NewMemberCreatedEvent(this));
    }

    public string UserId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? AboutInfo { get; private set; }
    public string? Address { get; private set; }
    public string? PEFriendCode { get; private set; }
    public string? PEUsername { get; private set; }

    public string? BlogUrl { get; private set; }
    public string? GitHubUrl { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? OtherUrl { get; private set; }
    public string? TwitchUrl { get; private set; }
    public string? YouTubeUrl { get; private set; }
    public string? TwitterUrl { get; private set; }
    public string? CodinGameUrl { get; private set; }
    public string? DiscordUsername { get; private set; }

    public List<Book>? BooksRead { get; set; } = new List<Book>();

    public DateTime DateCreated { get; private set; } = DateTime.UtcNow;
    public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public decimal? CityLatitude { get; set; }
    public decimal? CityLongitude { get; set; }


    public string UserFullName()
    {
      if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
      {
        return "[No Name Provided]";
      }
      else
      {
        return $"{FirstName} {LastName}";
      }
    }

    public void UpdateName(string? firstName, string? lastName)
    {
      bool valueChanged = false;
      if (FirstName != firstName)
      {
        FirstName = firstName;
        valueChanged = true;
      }
      if (LastName != lastName)
      {
        LastName = lastName;
        valueChanged = true;
      }
      if (valueChanged)
      {
        CreateOrUpdateUpdateEvent("Name");
      }
    }

    public void UpdateAddress(string? address)
    {
      if (Address == address) return;

      Address = address;
      CreateOrUpdateUpdateEvent(nameof(Address));
      UpdateMemberCityCoordinates();
    }

    public void UpdateMemberCityCoordinates()
    {
      var addressUpdatedEvent = new MemberAddressUpdatedEvent(this);
      Events.Add(addressUpdatedEvent);
    }

    public void UpdateAboutInfo(string? aboutInfo)
    {
      if (AboutInfo == aboutInfo) return;

      AboutInfo = aboutInfo;
      CreateOrUpdateUpdateEvent(nameof(AboutInfo));
    }
    public void UpdatePEInfo(string? peFriendCode, string? peUsername)
    {
      bool valueChanged = false;

      if (PEFriendCode != peFriendCode)
      {
        PEFriendCode = peFriendCode;
        valueChanged = true;
      }
      if (PEUsername != peUsername)
      {
        PEUsername = peUsername;
        valueChanged = true;
      }

      if (valueChanged)
      {
        CreateOrUpdateUpdateEvent("ProjectEuler");
      }
    }

    public void UpdateLinks(string? blogUrl,
      string? codinGameUrl,
        string? gitHubUrl,
        string? linkedInUrl,
        string? otherUrl,
        string? twitchUrl,
        string? youtubeUrl,
        string? twitterUrl)
    {
      bool valueChanged = false;
      if (BlogUrl != blogUrl)
      {
        BlogUrl = blogUrl;
        valueChanged = true;
      }
      if (CodinGameUrl != codinGameUrl)
      {
        CodinGameUrl = codinGameUrl;
        valueChanged = true;
      }
      if (GitHubUrl != gitHubUrl)
      {
        GitHubUrl = gitHubUrl;
        valueChanged = true;
      }
      if (LinkedInUrl != linkedInUrl)
      {
        LinkedInUrl = linkedInUrl;
        valueChanged = true;
      }
      if (OtherUrl != otherUrl)
      {
        OtherUrl = otherUrl;
        valueChanged = true;
      }
      if (TwitchUrl != twitchUrl)
      {
        TwitchUrl = twitchUrl;
        valueChanged = true;
      }
      if (YouTubeUrl != youtubeUrl)
      {
        YouTubeUrl = youtubeUrl;
        valueChanged = true;
      }
      if (TwitterUrl != twitterUrl)
      {
        TwitterUrl = twitterUrl;
        valueChanged = true;
      }
      if (valueChanged)
      {
        CreateOrUpdateUpdateEvent("Links");
      }
    }

    public void AddBookRead(Book book)
    {
      if (!(BooksRead!.Any(b => b.Id == book.Id)))
      {
        BooksRead.Add(book);
        CreateOrUpdateUpdateEvent("Books");
        var newBookReadEvent = new MemberAddedBookReadEvent(this, book);
        Events.Add(newBookReadEvent);
      }
    }

    public void RemoveBookRead(Book book)
    {
      if (BooksRead!.Any(b => b.Id == book.Id))
      {
        BooksRead.Remove(book);
        CreateOrUpdateUpdateEvent("Books");
      }
    }

    public void UpdateDiscord(string? discordUsername)
    {
      if (DiscordUsername == discordUsername) return;

      DiscordUsername = discordUsername;
      CreateOrUpdateUpdateEvent("Discord Username");
    }

    private void CreateOrUpdateUpdateEvent(string updateDetails)
    {
      MemberUpdatedEvent? updatedEvent = Events.Find(evt => evt is MemberUpdatedEvent) as MemberUpdatedEvent;

      if (updatedEvent != null)
      {
        updatedEvent.UpdateDetails += "," + updateDetails;
        return;
      }

      updatedEvent = new MemberUpdatedEvent(this, updateDetails);
      Events.Add(updatedEvent);
    }

    public int TotalSubscribedDays()
    {
      if (Subscriptions == null || !Subscriptions.Any()) return 0;

      return Subscriptions.Sum(s => s.Dates.ToDays(DateTime.Today));
    }
  }
}
