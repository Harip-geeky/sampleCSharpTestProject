using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var request = new MockRequest();
        var mediator = new MockMediator();
        var cancellationToken = CancellationToken.None;

        // --- THE MESSY LOGIC ---
        if (request.AddBudgetTransactionCommand.ExpenseType is not ExpenseType.BillPayment &&
            request.AddBudgetTransactionCommand.TransactionType is not TransactionType.CreditCardRefund and
            not TransactionType.DebitCardRefund &&
            !(request.AddBudgetTransactionCommand.BudgetTransactionType is BudgetTransactionType.DirectPayment &&
                request.AddBudgetTransactionCommand.Status == BudgetTransactionStatus.Void))
        {
            await mediator.Send(request.AddBudgetTransactionCommand, cancellationToken);
        }
        else if (request.AddBudgetTransactionCommand.ExpenseType is ExpenseType.BillPayment ||
               (request.AddBudgetTransactionCommand.BudgetTransactionType is BudgetTransactionType.DirectPayment &&
                request.AddBudgetTransactionCommand.Status == BudgetTransactionStatus.Void))
        {
            await ProcessBudgetTransactionRecalculationOnVoid(request, cancellationToken);
        }
    }

    static Task ProcessBudgetTransactionRecalculationOnVoid(MockRequest req, CancellationToken token) => Task.CompletedTask;
}

// --- MOCK STRUCTURES TO FIX BUILD ERRORS ---
public enum ExpenseType { BillPayment, General }
public enum TransactionType { CreditCardRefund, DebitCardRefund, Standard }
public enum BudgetTransactionType { DirectPayment, Manual }
public enum BudgetTransactionStatus { Void, Active }

public class MockRequest
{
    public Command AddBudgetTransactionCommand { get; set; } = new Command();
}

public class Command
{
    public ExpenseType ExpenseType { get; set; }
    public TransactionType TransactionType { get; set; }
    public BudgetTransactionType BudgetTransactionType { get; set; }
    public BudgetTransactionStatus Status { get; set; }
}

public class MockMediator
{
    public Task Send(object cmd, CancellationToken t) => Task.CompletedTask;
}