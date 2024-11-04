# Assignment: Extend the Combat System with Equipment Using Entity Framework Core and SOLID Principles

---

### **Objective**

This assignment aims to extend the existing combat system by implementing an **Equipment** system that includes **Weapons** and **Armor**. Using the principles of Entity Framework Core and SOLID, you will create additional entities and integrate them into your ConsoleRPG project. This assignment will build on your previous work with the `Player.cs` and `Goblin.cs` classes, enhancing the combat system to use these new features.

---

### **Instructions**

#### 1. Extend the `Player` Class (continuation from In-Class work)

- **Add Equipment Properties**: Create a new class, `Equipment`, that will contain properties for both `Weapon` and `Armor`. Ensure the `Player` class has a property for `Equipment`.
- **Integrate Weapon and Armor in Combat**:
  - Modify the `Attack` method in `Player` to factor in the `Weapon`’s attack power.
  - Ensure that the `Armor` property reduces the damage the player takes when attacked.
- **Entity Configuration**: Update setup in `GameContext` so that it supports the `Equipment` entity structure.
- **Migrations**: Create and apply migrations to update the database schema.

#### 2. Modify the `GameEngine` Class

- **Enhance Attack Logic**: In the `GameEngine` class, update the `AttackCharacter` method to:
  - Use the player’s `Weapon` attack value when attacking.
  - Calculate the effect of the player’s `Armor` to reduce incoming damage from the Goblin.
  - Ensure that the updated logic supports flexible changes in the player’s equipment.

#### 3. Configure the `GameContext` Class

- **Entity Framework Configuration**: Update `GameContext` to manage the new entities.
  - Ensure relationships are established correctly, using navigation properties where appropriate.
  - Set up any necessary one-to-one or one-to-many relationships between `Player` and `Equipment`.

---

### **Stretch Goal (+10%)**: Implement an `Item` Hierarchy with TPH

#### Goal
Create an abstract `Item` class, and derive `Weapon` and `Armor` classes from it. This will help modularize the game’s inventory system and make combat more extensible.

#### Steps for the Stretch Goal

1. **Create an `Item` Abstract Class** (10 points)
   - Define common properties such as `Name`, `Value`, and `Description`.
   - Include an abstract `Use` method to allow custom behavior for different item types.

2. **Concrete Classes for `Weapon` and `Armor`** (10 points)
   - **Weapon Class**: Define properties such as `AttackPower` and `Durability`.
   - **Armor Class**: Define properties like `DefenseRating` and `Durability`.
   - Implement specific behavior for `Use` in each subclass.

3. **Modify `Player` and `Goblin` to Use `Item` Hierarchy** (10 points)
   - Update `Player` to hold an `Item` collection, associating specific items (e.g., a sword or shield) for dynamic inventory.
   - Allow `Goblin` to drop an item upon defeat, or have them interact with `Player` items in combat.

4. **Update `GameContext`** (10 points)
   - Configure TPH in `GameContext` for the `Item` hierarchy using `.HasDiscriminator()`.
   - Ensure relationships between `Player` and their items are set up for many-to-many with an intermediary table if needed.

---

### **Grading Criteria**

| Task                                           | Points |
|------------------------------------------------|--------|
| Player Class Modifications                      | 20     |
| Equipment Integration in Classes                | 20     |
| GameEngine Modifications                        | 20     |
| GameContext Configuration                       | 20     |
| **Stretch Goal**                                | **+10**  |
| **Total Possible Points**                       | 100 (+10%) |

---

### **Reminders**

1. **Connection Strings**: 
   - Set up the connection string in your `appsettings.json` or `ConfigurationHelper` file.
   - Use [SQL Server connection strings](https://www.connectionstrings.com/sql-server/) as a reference.

2. **Migrations**:
   - To create the initial migration:
     ```bash
     dotnet ef migrations add InitialCreate
     ```
   - To update the database:
     ```bash
     dotnet ef database update
     ```

3. **Testing**:
   - Ensure your game runs correctly after implementing the changes.
   - Test all functionalities, especially new combat modifications.

---

**Good luck, and enjoy enhancing the ConsoleRPG combat experience!**