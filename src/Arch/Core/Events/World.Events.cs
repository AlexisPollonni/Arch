#define EVENTS

using Arch.Core.Utils;
using MessagePipe;

// ReSharper disable once CheckNamespace
namespace Arch.Core;

public partial class World
{
    public readonly record struct EntityCreatedArgs(Entity Entity);
    public readonly record struct EntityDestroyedArgs(Entity Entity);
    public readonly record struct ComponentAddedArgs(ComponentType Type, Entity Entity);
    public readonly record struct ComponentRemovedArgs(ComponentType Type, Entity Entity);
    public readonly record struct ComponentSetArgs(ComponentType Type, Entity Entity);


    public ISubscriber<EntityCreatedArgs> EntityCreated => GlobalMessagePipe.GetSubscriber<EntityCreatedArgs>();
    public ISubscriber<EntityDestroyedArgs> EntityDestroyed => GlobalMessagePipe.GetSubscriber<EntityDestroyedArgs>();

    public ISubscriber<ComponentAddedArgs> ComponentAdded => GlobalMessagePipe.GetSubscriber<ComponentAddedArgs>();
    public ISubscriber<ComponentRemovedArgs> ComponentRemoved => GlobalMessagePipe.GetSubscriber<ComponentRemovedArgs>();
    public ISubscriber<ComponentSetArgs> ComponentSet => GlobalMessagePipe.GetSubscriber<ComponentSetArgs>();


    private readonly IPublisher<EntityCreatedArgs> _entityCreatePublisher = GlobalMessagePipe.GetPublisher<EntityCreatedArgs>();
    private readonly IPublisher<EntityDestroyedArgs> _entityDestroyPublisher = GlobalMessagePipe.GetPublisher<EntityDestroyedArgs>();

    private readonly IPublisher<ComponentAddedArgs> _compAddPublisher = GlobalMessagePipe.GetPublisher<ComponentAddedArgs>();
    private readonly IPublisher<ComponentRemovedArgs> _compRemovePublisher = GlobalMessagePipe.GetPublisher<ComponentRemovedArgs>();
    private readonly IPublisher<ComponentSetArgs> _compSetPublisher = GlobalMessagePipe.GetPublisher<ComponentSetArgs>();


    /// <summary>
    ///     Calls all handlers subscribed to entity creation.
    /// </summary>
    /// <param name="entity">The entity that got created.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnEntityCreated(Entity entity)
    {
#if EVENTS
        _entityCreatePublisher.Publish(new(entity));
#endif
    }

    /// <summary>
    ///     Calls all handlers subscribed to entity deletion.
    /// </summary>
    /// <param name="entity">The entity that got destroyed.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnEntityDestroyed(Entity entity)
    {
#if EVENTS
        _entityDestroyPublisher.Publish(new(entity));
#endif
    }

    /// <summary>
    ///     Calls all generic handlers subscribed to component addition of this type.
    /// </summary>
    /// <param name="entity">The entity that the component was added to.</param>
    /// <typeparam name="T">The type of component that got added.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnComponentAdded<T>(Entity entity)
    {
#if EVENTS
        var type = Component.GetComponentType(typeof(T));
        OnComponentAdded(entity, type);
#endif
    }

    /// <summary>
    ///     Calls all generic handlers subscribed to component setting of this type.
    /// </summary>
    /// <param name="entity">The entity that the component was set on.</param>
    /// <typeparam name="T">The type of component that got set.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnComponentSet<T>(Entity entity)
    {
#if EVENTS
        var type = Component.GetComponentType(typeof(T));
        OnComponentSet(entity, type);
#endif
    }

    /// <summary>
    ///     Calls all generic handlers subscribed to component removal.
    /// </summary>
    /// <param name="entity">The entity that the component was removed from.</param>
    /// <typeparam name="T">The type of component that got removed.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnComponentRemoved<T>(Entity entity)
    {
#if EVENTS
        var type = Component.GetComponentType(typeof(T));
        OnComponentRemoved(entity, type);
#endif
    }

    /// <summary>
    ///     Calls all handlers subscribed to component addition of this type.
    /// </summary>
    /// <param name="entity">The entity that the component was added to.</param>
    /// <param name="compType">The type of component that got added.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnComponentAdded(Entity entity, ComponentType compType)
    {
#if EVENTS
        _compAddPublisher.Publish(new(compType, entity));
#endif
    }

    /// <summary>
    ///     Calls all handlers subscribed to component setting of this type.
    /// </summary>
    /// <param name="entity">The entity that the component was set on.</param>
    /// <param name="comp">The component instance that got set.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnComponentSet(Entity entity, object comp)
    {
#if EVENTS
        var cType = Component.GetComponentType(comp.GetType());
        _compSetPublisher.Publish(new(cType, entity));
#endif
    }

    /// <summary>
    ///     Calls all handlers subscribed to component removal.
    /// </summary>
    /// <param name="entity">The entity that the component was removed from.</param>
    /// <param name="compType">The type of component that got removed.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnComponentRemoved(Entity entity, ComponentType compType)
    {
#if EVENTS
        _compRemovePublisher.Publish(new(compType, entity));
#endif
    }

    /// <summary>
    ///     Calls all handlers subscribed to component addition of this type for entities in a archetype range.
    /// </summary>
    /// <param name="archetype">The <see cref="Archetype"/>.</param>
    /// <typeparam name="T">The component type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void OnComponentAdded<T>(Archetype archetype)
    {
#if EVENTS
        // Set the added component, start from the last slot and move down
        foreach (ref var chunk in archetype)
        {
            ref var firstEntity = ref chunk.Entity(0);
            foreach (var index in chunk)
            {
                var entity = Unsafe.Add(ref firstEntity, index);
                OnComponentAdded<T>(entity);
            }
        }
#endif
    }

    /// <summary>
    ///     Calls all handlers subscribed to component removal of this type for entities in a archetype range.
    /// </summary>
    /// <param name="archetype">The <see cref="Archetype"/>.</param>
    /// <typeparam name="T">The component type.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void OnComponentRemoved<T>(Archetype archetype)
    {
#if EVENTS
        // Set the added component, start from the last slot and move down
        foreach (ref var chunk in archetype)
        {
            ref var firstEntity = ref chunk.Entity(0);
            foreach (var index in chunk)
            {
                var entity = Unsafe.Add(ref firstEntity, index);
                OnComponentRemoved<T>(entity);
            }
        }
#endif
    }
}
