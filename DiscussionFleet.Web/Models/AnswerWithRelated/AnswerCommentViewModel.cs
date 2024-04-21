using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.CommentAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Helpers;
using DiscussionFleet.Web.Utils;

namespace DiscussionFleet.Web.Models.AnswerWithRelated;

public class AnswerCommentViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IDateTimeProvider _dateTimeProvider;
    private IGuidProvider _guidProvider;

    public AnswerCommentViewModel()
    {
    }

    public AnswerCommentViewModel(IApplicationUnitOfWork appUnitOfWork, ILifetimeScope scope,
        IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider)
    {
        _appUnitOfWork = appUnitOfWork;
        _scope = scope;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
    }


    public Guid QuestionId { get; set; }
    public Guid AnswerId { get; set; }

    [Required]
    [StringLength(DomainEntityConstants.CommentBodyMaxLength,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = DomainEntityConstants.CommentBodyMinLength)]
    [Display(Name = "Body")]

    public string Body { get; set; }


    public async Task<bool> ConductCommentCreateAsync(Guid questionId, Guid answerId, Guid commentWriterId)
    {
        var answer = await _appUnitOfWork.AnswerRepository.GetOneAsync(
            filter: x => x.Id == answerId && x.QuestionId == questionId,
            includes: [i => i.Comments],
            useSplitQuery: false
        );

        if (answer is null) return false;


        var comment = new Comment
        {
            Id = _guidProvider.SortableGuid(),
            Body = Body,
            CommenterId = commentWriterId,
        };

        comment.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.CommentRepository.CreateAsync(comment);


        var questionComment = new AnswerComment
        {
            AnswerId = answer.Id,
            CommentId = comment.Id
        };

        questionComment.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
        answer.Comments.Add(questionComment);

        await _appUnitOfWork.SaveAsync();

        return true;
    }


    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _dateTimeProvider = _scope.Resolve<IDateTimeProvider>();
        _guidProvider = _scope.Resolve<IGuidProvider>();
    }
}