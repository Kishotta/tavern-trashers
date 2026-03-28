# Good and Bad Tests

## Good Tests

**Integration-style**: Test through real interfaces, not mocks of internal parts.

```csharp
// GOOD: Tests observable behavior
[Fact]
public async Task UserCanCheckoutWithValidCart() 
{
    var cart = Cart.Create();
    cart.Add(product);
    var result = await CheckoutAsync(cart, paymentMethod);
    result.Status.ShouldBe("Confirmed");
}
```

Characteristics:

- Tests behavior users/callers care about
- Uses public API only
- Survives internal refactors
- Describes WHAT, not HOW
- One logical assertion per test

## Bad Tests

**Implementation-detail tests**: Coupled to internal structure.

```csharp
// BAD: Tests implementation details
[Fact]
public async Task CheckoutCallsPaymentServiceProcess() 
{
    var mockPayment = Substitute.For<PaymentService>();
    await Checkout(cart, mockPayment);
    payment.Received(1).Process(cart.Total);
}
```

Red flags:

- Mocking internal collaborators
- Testing private methods
- Asserting on call counts/order
- Test breaks when refactoring without behavior change
- Test name describes HOW not WHAT
- Verifying through external means instead of interface

```typescript
// BAD: Bypasses interface to verify
test("createUser saves to database", async () => {
  await createUser({ name: "Alice" });
  const row = await db.query("SELECT * FROM users WHERE name = ?", ["Alice"]);
  expect(row).toBeDefined();
});

// GOOD: Verifies through interface
test("createUser makes user retrievable", async () => {
  const user = await createUser({ name: "Alice" });
  const retrieved = await getUser(user.id);
  expect(retrieved.name).toBe("Alice");
});
```