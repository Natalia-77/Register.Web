using Domain;
using FluentValidation;
using Register.Web.Models;

namespace Register.Web.Validation
{
    public class UserValidator : AbstractValidator<RegisterViewModel>
    {
        private readonly AppDbContext _dbContext;

        public UserValidator(AppDbContext dbContext)
        {
            _dbContext = dbContext;

           
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Поле не може бути пустим");

            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Поле не може бути пустим!")
              .EmailAddress().WithMessage("Пароль має містити '@'")
              .DependentRules(() =>
              {
                  RuleFor(x => x.Email)
                  .Must(IsNotExist).WithName("Email").WithMessage("Такий користувач вже існує!");
              });

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Поле не може бути пустим  ")
               .Length(5, 100).WithMessage("Пароль не може бути менше 5 символів")
               .Matches(@"\d").WithName("Password").WithMessage("Пароль повинен містити хоча б одну цифру");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Поле не може бути пустим")
                .Equal(x => x.Password).WithMessage("Пароль і підтверджений пароль не співпадають");

        }

        private bool IsNotExist(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                return true;
            }
            return false;
        }
    }
}
