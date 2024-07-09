using System.Linq.Expressions;
using DiscussionFleet.Application;
using DiscussionFleet.Application.AnswerFeatures;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.ResourceNotificationFeatures;
using DiscussionFleet.Application.VotingFeatures;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Web.Models.AnswerWithRelated;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace DiscussionFleet.Tests.Web;

public class AnswerViewModelTests
{
    private Mock<IApplicationUnitOfWork> _appUnitOfWorkMock;
    private Mock<IResourceNotificationService> _resourceNotificationService;
    private Mock<IAnswerService> _answerServiceMock;
    private Mock<IVotingService> _votingServiceMock;
    private Mock<IOptions<ForumRulesOptions>> _forumRulesOptionsMock;
    private AnswerViewModel _viewModel;

    [SetUp]
    public void SetUp()
    {
        _appUnitOfWorkMock = new Mock<IApplicationUnitOfWork>();
        _answerServiceMock = new Mock<IAnswerService>();
        _resourceNotificationService = new Mock<IResourceNotificationService>();
        _votingServiceMock = new Mock<IVotingService>();
        _forumRulesOptionsMock = new Mock<IOptions<ForumRulesOptions>>();
        _forumRulesOptionsMock.Setup(f => f.Value).Returns(new ForumRulesOptions()
        {
            MinimumReputationForVote = 1,
            QuestionVotedUp = 1,
            AnswerVotedUp = 1,
            AnswerMarkedAccepted = 1,
            QuestionVotedDown = 1,
            AnswerVotedDown = 1,
            NewQuestion = 1,
            NewAnswer = 10
        });
        _viewModel = new AnswerViewModel(_appUnitOfWorkMock.Object, _answerServiceMock.Object,
            _votingServiceMock.Object, _forumRulesOptionsMock.Object, _resourceNotificationService.Object);
    }


    [Test]
    public async Task IsQuestionExistsAsync_QuestionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        _appUnitOfWorkMock.Setup(u => u.QuestionRepository.GetOneAsync(
            It.IsAny<Expression<Func<Question, bool>>>(),
            It.IsAny<bool>(),
            It.IsAny<ICollection<Expression<Func<Question, object?>>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((Question)null);

        // Act
        var result = await _viewModel.IsQuestionExistsAsync(Guid.NewGuid(), questionId);

        // Assert
        result.ShouldBeFalse();
    }
}