﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniSpace.Services.Events.Core.Exceptions;

namespace MiniSpace.Services.Events.Core.Entities
{
    public class Event: AggregateRoot
    {
        private ISet<Organizer> _coOrganizers = new HashSet<Organizer>();
        private ISet<Participant> _interestedStudents = new HashSet<Participant>();
        private ISet<Participant> _signedUpStudents = new HashSet<Participant>();
        private ISet<Rating> _ratings = new HashSet<Rating>();
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Organizer Organizer { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public Address Location { get; private set; }
        //public string Image { get; set; }
        public int Capacity { get; private set; }
        public decimal Fee { get; private set; }
        public Category Category { get; private set; }
        public State State { get; private set; }
        public DateTime PublishDate { get; private set; }
        
        public IEnumerable<Organizer> CoOrganizers
        {
            get => _coOrganizers;
            private set => _coOrganizers = new HashSet<Organizer>(value);
        }
        
        public IEnumerable<Participant> InterestedStudents
        {
            get => _interestedStudents;
            private set => _interestedStudents = new HashSet<Participant>(value);
        }
        
        public IEnumerable<Participant> SignedUpStudents
        {
            get => _signedUpStudents;
            private set => _signedUpStudents = new HashSet<Participant>(value);
        }
        
        public IEnumerable<Rating> Ratings
        {
            get => _ratings;
            private set => _ratings = new HashSet<Rating>(value);
        }

        public Event(AggregateId id,  string name, string description, DateTime startDate, DateTime endDate, 
            Address location, int capacity, decimal fee, Category category, State state, DateTime publishDate,
            Organizer organizer, IEnumerable<Organizer> coOrganizers = null, IEnumerable<Participant> interestedStudents = null, 
            IEnumerable<Participant> signedUpStudents = null, IEnumerable<Rating> ratings = null)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Location = location;
            Capacity = capacity;
            Fee = fee;
            Category = category;
            State = state;
            Organizer = organizer;
            CoOrganizers = coOrganizers ?? Enumerable.Empty<Organizer>();
            InterestedStudents = interestedStudents ?? Enumerable.Empty<Participant>();
            SignedUpStudents = signedUpStudents ?? Enumerable.Empty<Participant>();
            Ratings = ratings ?? Enumerable.Empty<Rating>();
            PublishDate = publishDate;
        }
        
        public static Event Create(AggregateId id,  string name, string description, DateTime startDate, DateTime endDate, 
            Address location, int capacity, decimal fee, Category category, State state, DateTime publishDate, Organizer organizer)
        {
            var @event = new Event(id, name, description, startDate, endDate, location, capacity, fee, category, 
                state, publishDate, organizer);
            return @event;
        }
        
        public void AddOrganizer(Organizer organizer)
        {
            if (CoOrganizers.Any(o => o.Id == organizer.Id))
            {
                throw new OrganizerAlreadyAddedException(organizer.Id);
            }

            _coOrganizers.Add(organizer);
        }
        
        public void SignUpStudent(Participant participant)
        {
            if (SignedUpStudents.Any(p => p.StudentId == participant.StudentId))
            {
                throw new StudentAlreadySignedUpException(participant.StudentId, Id);
            }

            if (SignedUpStudents.Count() >= Capacity)
            {
                throw new EventCapacityExceededException(Id, Capacity);
            }

            _signedUpStudents.Add(participant);
        }
        
        public void ShowStudentInterest(Participant participant)
        {
            if (InterestedStudents.Any(p => p.StudentId == participant.StudentId))
            {
                throw new StudentAlreadyInterestedInEventException(participant.StudentId, Id);
            }

            _interestedStudents.Add(participant);
        }
        
        public void Rate(Guid studentId, int rating)
        {
            if(_signedUpStudents.All(p => p.StudentId != studentId))
            {
                throw new StudentNotSignedUpForEventException(Id ,studentId);
            }
            
            if (rating < 1 || rating > 5)
            {
                throw new InvalidRatingValueException(rating);
            }

            if (_ratings.Any(r => r.StudentId == studentId))
            {
                throw new StudentAlreadyRatedEventException(Id, studentId);
            }

            _ratings.Add(new Rating(studentId, rating));
        }

        public bool UpdateState(DateTime now)
        {
            if(State == State.ToBePublished && PublishDate <= now)
            {
                ChangeState(State.Published);
            }
            else if (State == State.Published && EndDate <= now)
            {
                ChangeState(State.Archived);
            }
            else
                return false;

            return true;
        }
        
        private void ChangeState(State state)
        {
            if (State == state)
            {
                return;
            }

            State = state;
        }
    }
}