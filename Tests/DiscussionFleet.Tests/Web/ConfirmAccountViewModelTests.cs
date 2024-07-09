using Autofac.Extras.Moq;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SharpOutcome;
using Shouldly;

namespace DiscussionFleet.Tests.Web
{
    public class ConfirmAccountViewModelTests
    {
        private AutoMock _mock;
        private ConfirmAccountViewModel _viewModel;
        private Mock<IMemberService> _memberServiceMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;

        [TearDown]
        public void TearDown()
        {
            _mock.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _mock = AutoMock.GetLoose();
            _memberServiceMock = _mock.Mock<IMemberService>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = Array.Empty<IUserValidator<ApplicationUser>>();
            var passwordValidators = Array.Empty<IPasswordValidator<ApplicationUser>>();
            var normalizerMock = new Mock<ILookupNormalizer>();
            var errorDescriberMock = new Mock<IdentityErrorDescriber>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<ApplicationUser>>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, optionsMock.Object, passwordHasherMock.Object,
                userValidators, passwordValidators, normalizerMock.Object,
                errorDescriberMock.Object, serviceProviderMock.Object, loggerMock.Object);

            _viewModel = _mock.Create<ConfirmAccountViewModel>();
        }

        [Test]
        public async Task ConductConfirmationAsync_UserNotFound_ReturnsBadOutcome()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _viewModel.ConductConfirmationAsync("id", "code");

            // Assert
            result.ShouldBeOfType<Outcome<ApplicationUser, IBadOutcome>>();
            
            if (result.TryPickBadOutcome(out var err))
            {
                err.Tag.ShouldBe(BadOutcomeTag.NotFound);
            }
        }

        [Test]
        public async Task ConductConfirmationAsync_UserAlreadyVerified_ReturnsBadOutcome()
        {
            // Arrange
            var user = new ApplicationUser { EmailConfirmed = true };
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var result = await _viewModel.ConductConfirmationAsync("id", "code");

            // Assert
            result.ShouldBeOfType<Outcome<ApplicationUser, IBadOutcome>>();
            
            if (result.TryPickBadOutcome(out var err))
            {
                err.Tag.ShouldBe(BadOutcomeTag.NotFound);
            }
        }

        [Test]
        public async Task ConductConfirmationAsync_InvalidVerificationCode_ReturnsBadOutcome()
        {
            // Arrange
            var user = new ApplicationUser { EmailConfirmed = false };
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _memberServiceMock.Setup(m => m.ConfirmEmailAsync(user, It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _viewModel.ConductConfirmationAsync("id", "code");

            result.ShouldBeOfType<Outcome<ApplicationUser, IBadOutcome>>();
            // Assert
            if (result.TryPickBadOutcome(out var err))
            {
                err.Tag.ShouldBe(BadOutcomeTag.NotFound);
            }
        }

        [Test]
        public async Task ConductConfirmationAsync_ValidVerificationCode_ReturnsUser()
        {
            // Arrange
            var user = new ApplicationUser { EmailConfirmed = false };
            _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _memberServiceMock.Setup(m => m.ConfirmEmailAsync(user, It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _viewModel.ConductConfirmationAsync("id", "code");

            // Assert
            result.ShouldBeOfType<Outcome<ApplicationUser, IBadOutcome>>();
            
            if (result.TryPickGoodOutcome(out var usr))
            {
                result.ShouldBe(usr);
            }
        }
    }
}