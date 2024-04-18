using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.ResourceNotificationFeatures;
using DiscussionFleet.Domain.Entities.Enums;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Web.Utils;

namespace DiscussionFleet.Web.Models.Others;

public class QuestionCommentViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IDateTimeProvider _dateTimeProvider;
    private IGuidProvider _guidProvider;
    private IResourceNotificationService _resourceNotificationService;

    public QuestionCommentViewModel()
    {
    }

    public QuestionCommentViewModel(IApplicationUnitOfWork appUnitOfWork, ILifetimeScope scope,
        IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider,
        IResourceNotificationService resourceNotificationService)
    {
        _appUnitOfWork = appUnitOfWork;
        _scope = scope;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _resourceNotificationService = resourceNotificationService;
    }


    public Guid Id { get; set; }

    [Required]
    [StringLength(DomainEntityConstants.CommentBodyMaxLength,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = DomainEntityConstants.CommentBodyMinLength)]
    [Display(Name = "Body")]

    public string Body { get; set; }


    public async Task<bool> ConductCommentCreateAsync(Guid id, Guid commentWriterId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == id,
            includes: [i => i.Comments],
            useSplitQuery: false
        );

        if (question is null) return false;


        var comment = new Comment
        {
            Id = _guidProvider.SortableGuid(),
            Body = Body,
            CommenterId = commentWriterId,
        };

        comment.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.CommentRepository.CreateAsync(comment);


        var questionComment = new QuestionComment
        {
            QuestionId = question.Id,
            CommentId = comment.Id
        };

        questionComment.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
        question.Comments.Add(questionComment);

        await _appUnitOfWork.SaveAsync();

        await _resourceNotificationService.NotifyQuestionAuthorAsync(
            question.AuthorId, question.Id, question.Title,
            ResourceNotificationType.Comment
        );

        return true;
    }


    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _dateTimeProvider = _scope.Resolve<IDateTimeProvider>();
        _guidProvider = _scope.Resolve<IGuidProvider>();
        _resourceNotificationService = _scope.Resolve<IResourceNotificationService>();
    }
}