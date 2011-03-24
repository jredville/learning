/*
 * Reactive Extensions to .NET (Rx)
 * bartde - April 2010 
 * 
 * Download Rx from MSDN DevLabs.
 */

using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Disposables;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rx40Samples
{
    class Program
    {
        static void Main()
        {
            var mnu = (from mtd in typeof(Program).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                       where mtd.Name != "Main"
                       where !mtd.Name.StartsWith("<") // compiler-generated stuff
                       where !mtd.Name.StartsWith("_") // for demos, allows to hide a sample temporarily
                       select new { mtd.Name, Call = new Action(() => mtd.Invoke(null, null)) })
                      .ToList();

            var get = new Func<string, Func<int, bool>, int>((prompt, check) =>
            {
                while (true)
                {
                    Console.Write(prompt);
                    var res = default(int);
                    if (int.TryParse(Console.ReadLine(), out res) && check(res))
                        return res;
                }
            });

            while (true)
            {
                Console.WindowHeight = 6 + mnu.Count;

                Console.Clear();

                Console.WriteLine("Rx samples :: bartde - April 2010");
                Console.WriteLine("---------------------------------");
                Console.WriteLine();

                Console.WriteLine("Pick your Rx poison:");

                var i = 1;
                foreach (var opt in mnu)
                    Console.WriteLine("{0}. {1}", i++, opt.Name);
                Console.WriteLine();

                var c = get("Your choice: ", x => x >= 0 && x <= mnu.Count);
                if (c == 0)
                    break;

                var s = mnu[c - 1];

                Console.Clear();
                Console.WriteLine(s.Name);
                Console.WriteLine();
                s.Call();
                Console.WriteLine();
                Console.WriteLine("Sample finished. Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void Essence()
        {
            //
            // Observables are the dual to enumerables. They allow for a subscription, given
            // an observer object (which is the dual to an enumerator). One way to create an
            // observable sequence is by implementing IObservable<T>. Alternatively, one can
            // use factory methods like Observable.Create[WithDisposable] or Observable.Defer:
            //
            var lottery = Observable.Create<int>(observer =>
            {
                var rand = new Random();
                var stop = false;
                var quit = new ManualResetEvent(false);

                new Thread(() =>
                {
                    //
                    // The grammar of messages that can be sent out is as follows:
                    //   OnNext* (OnError | OnCompleted)
                    //
                    // Exceptions are terminating and have immediate abort semantics (as
                    // the CLR does in general).
                    //
                    while (!stop)
                    {
                        observer.OnNext(rand.Next());
                        Thread.Sleep(1000);
                    }
                    quit.Set();
                }).Start();

                //
                // When using CreateWithDisposable, you could create the required IDisposable
                // object by using Disposable.Create:
                //   return Disposable.Create(() => { ... });
                //
                return () => { stop = true; quit.WaitOne(); };
            });

            //
            // To subscribe to an observable sequence, use the Subscribe method or one of its
            // extension methods. Ultimately, Subscribe wants an observer object which can
            // respond to the triad of supported messages. Default actions exist when certain
            // handlers are omitted (e.g. no OnError will cause the exception to propagate).
            //
            var subscription = 
                lottery.Subscribe(
                    // OnNext
                    x => Console.WriteLine(x),
                    // OnError
                    ex => Console.WriteLine("Error: " + ex.Message),
                    // OnCompleted
                    () => Console.WriteLine("Done!")
                );

            //
            // Lottery is going on a background thread, causing the handlers specified
            // above to be called (on that thread). See ObserveOn and SubscribeOn for ways
            // to migrate effects to another "concurrency context" (IScheduler interface).
            //
            Thread.Sleep(5000);

            //
            // To unsubscribe, call Dispose on the subscription IDisposable object. This
            // will take care of all the unhooking of handlers required.
            //
            subscription.Dispose();
        }

        static void Creation()
        {
            //
            // Various IObservable<T> creation operators exist, which are illustrated below.
            // We'll create an observer to illustrate all of them in a uniform manner. The
            // use of the Run operator will block till an OnError or OnCompleted message is
            // received.
            //
            var obs = new Func<IObserver<int>>(() => Observer.Create<int>(
                x  => Console.WriteLine(x),
                ex => Console.WriteLine("Error: " + ex.Message),
                () => Console.WriteLine("Done!")
            ));

            //
            // Runner with pretty printing.
            //
            var run = new Action<string, IObservable<int>>((title, src) =>
            {
                Console.WriteLine(title);
                src.Run(obs());
                Console.WriteLine();
            });

            //
            // 0. Empty
            //
            var eps = Observable.Empty<int>();
            run("Empty", eps);

            //
            // 1. Return
            //
            var ret = Observable.Return(1);
            run("Return", ret);

            //
            // 2. Throw
            //
            var err = Observable.Throw<int>(new Exception("Oops!"));
            run("Throw", err);

            //
            // 3. StartWith (LISP's cons in reverse)
            //
            var cns = Observable.Return(1).StartWith(0);
            run("StartWith", cns);

            //
            // 4. Range
            //
            var rng = Observable.Range(0, 10);
            run("Range", rng);

            //
            // 5. Concat
            //
            var con = Observable.Range(0, 10).Concat(Observable.Range(10, 10));
            run("Concat", con);

            //
            // 6. Repeat
            //
            var rpt = Observable.Range(0, 2).Repeat(3);
            run("Repeat", rpt);

            //
            // 7. Generate (anamorphic) operators
            //
            var gen = Observable.Generate(0, i => i < 10, i => i * i, i => i + 1);
            run("Generate", gen);
            var tmr = Observable.GenerateWithTime(0, i => i < 10, i => i * i, i => TimeSpan.FromSeconds(i), i => i + 1);
            run("GenerateWithTime", tmr);

            //
            // ∞. Never
            //
            var inf = Observable.Never<int>();
            Console.WriteLine("Never");
            inf.Subscribe(x => Console.WriteLine("How did this happen?"));
        }

        static void MouseMoveEvents()
        {
            var frm = new Form();

            //
            // FromEvent bridges classic .NET events with IObservable<T>. The result uses an
            // IEvent<TEventArgs>, as specified on the generic parameter. This interface wraps
            // both the Sender and EventArgs of the event. So the below is inferred to be of
            // type IObservable<IEvent<MouseEventArgs>>.
            //
            var mme = Observable.FromEvent<MouseEventArgs>(frm, "MouseMove");

            //
            // Subscribe has a couple of overloads, most of which are extension methods.
            //
            // The master method's signature is IDisposable Subscribe(IObserver<T> observer).
            // Due to the lack of anonymous types (as they exist in Java), we provide overloads
            // that take in the triad of IObserver<T> functions, i.e. OnNext, OnError, OnCompleted.
            // Here we're only specifying an OnNext handler.
            //
            using (mme.Subscribe(evt => Console.WriteLine(evt.EventArgs.Location)))
            {
                Application.Run(frm);

                //
                // When we reach the curly brace, the IDisposable.Dispose method is called, causing
                // an unsubscription from the event.
                //
            }
        }

        static void ColdOrHot()
        {
            //
            // Cold observables start ticking upon subscription. Therefore they can have a
            // repeatable characteristic. An example is an IEnumerable being converted to
            // an IObservable. The underlying enumerable is cold by itself and AsObservable
            // keeps that property invariant.
            //
            var rng = Enumerable.Range(0, 5).ToObservable();
            rng.Run(Console.WriteLine);
            
            //
            // Run is a synchronous version of Subscribe, blocking till either OnError or
            // OnCompleted gets signaled. Basically the below corresponds to the following:
            // 
            //   var evt = new ManualResetEvent(false);
            //   rng.Subscribe(Console.WriteLine, ex => evt.Set(), () => evt.Set());
            //   evt.WaitOne();
            //
            rng.Run(Console.WriteLine); // Hasn't run out of juice.

            //
            // Hot observables are typically generated by the system, e.g. on a background
            // thread. There's no way to hold them till a subscription happens. Even before
            // the subscription has taken place, the events are flowing already; we're just
            // not listening to them yet. An example is the MouseMove event.
            //
            var btn1 = new Button { Text = "Subscribe 1" };
            var btn2 = new Button { Text = "Subscribe 2", Left = btn1.Left + btn1.Width + 10 };
            var frm = new Form
            {
                Controls = { btn1, btn2 }
            };

            var mme = Observable.FromEvent<MouseEventArgs>(frm, "MouseMove")
                .Select(evt => evt.EventArgs.Location);

            //
            // In between the declaration of the event and the subscription, we can't at
            // all prevent the mouse from moving (despite ongoing efforts to provide auto-
            // desktop-glue-capable devices).
            //
            btn1.Click += (o, e) => mme.Subscribe(loc => Console.WriteLine("First:  " + loc));
            btn2.Click += (o, e) => mme.Subscribe(loc => Console.WriteLine("Second: " + loc));
            Application.Run(frm);

            //
            // Notice some advanced operators like Publish can turn a cold sequence into a
            // hot one.
            //
        }

        static void MouseCursorOverFirstBisector()
        {
            var frm = new Form();

            //
            // We get LINQ query comprehension syntax for free, as we implement the Standard
            // Query Operators, such as Select and Where which are used below.
            //
            var mme = from mm in Observable.FromEvent<MouseEventArgs>(frm, "MouseMove")
                      select mm.EventArgs.Location;

            //
            // Notice LINQ is not tied to IEnumerable<T> or so. In fact, the type of the res
            // variable below will be inferred to be IObservable<Point>.
            //
            var res = from mm in mme
                      where mm.X == mm.Y
                      select mm;

            //
            // Using so-called eta-reduction (a term from lambda calculus), the below can be
            // simplified to res.Subscribe(Console.WriteLine), but we use the more elaborate
            // form to illustrate the OnNext handler that's fed to Subscribe.
            //
            res.Subscribe(point => Console.WriteLine(point));

            Application.Run(frm);
        }

        static void MouseCursorInNorthWest()
        {
            //
            // Simple form with four quadrants indicated by red lines.
            //
            var frm = new Form();
            frm.Paint += (o, e) =>
            {
                var w = frm.ClientSize.Width / 2;
                var h = frm.ClientSize.Height / 2;

                e.Graphics.DrawLine(Pens.Red, w, 0, w, 10000);
                e.Graphics.DrawLine(Pens.Red, 0, h, 10000, h);
            };

            var mme = from mm in Observable.FromEvent<MouseEventArgs>(frm, "MouseMove")
                      select mm.EventArgs.Location; 

            //
            // Use of "let" causes the use of multiple subsequent Select operators
            // which introduce a "scope" in the query using anonymous types that
            // contain the bindings. In this case, w and h are reevaluated every
            // time a mouse move happens, which accommodates for form resizes.
            //
            var res = from mm in mme
                      let w = frm.ClientSize.Width
                      let h = frm.ClientSize.Height
                      where mm.X < w / 2 && mm.Y < h / 2
                      select mm;

            res.Subscribe(point => Console.WriteLine(point));

            Application.Run(frm);
        }

        static void GroupMouseMovesByQuadrant()
        {
            var frm = new Form();
            frm.Paint += (o, e) =>
            {
                var w = frm.ClientSize.Width / 2;
                var h = frm.ClientSize.Height / 2;

                e.Graphics.DrawLine(Pens.Red, w, 0, w, 10000);
                e.Graphics.DrawLine(Pens.Red, 0, h, 10000, h);
            };

            //
            // We use some projection to shake off all the nuisance event information provided
            // by .NET events, producing an IObservable<Point> instead. The use of Do allows
            // us to evaluate a lambda expression for its side-effects, in this case to facilitate
            // logging whenever a MouseMove event is triggered.
            //
            var mme = from evt in Observable.FromEvent<MouseEventArgs>(frm, "MouseMove")
                          .Do(x => Console.WriteLine("!"))
                      select evt.EventArgs.Location;

            //
            // Four quadrants can be identified by a simple calculation:
            //
            // +-------+-------+
            // |   0   |   1   |
            // |       |       |
            // +-------+-------+
            // |   2   |   3   |
            // |       |       |
            // +-------+-------+
            //
            // Use of the GroupBy operator transforms an IObservable<T> into an
            // IObservable<IGroupedObservable<K, T>> where K stands for the key
            // type and T for the element type.
            //
            (from pt in mme
             let w = frm.ClientSize.Width / 2
             let h = frm.ClientSize.Height / 2
             group pt by (pt.X / w) + (pt.Y / h) * 2)
            .Subscribe(g =>
            {
                //
                // The creation of groups is lazy; as soon as new key value is
                // encountered, the outer resultant observable will trigger an
                // OnNext message signaling the creation of a new group. In here
                // we can subscribe to that group.
                //
                // Notice this is exactly the dual to the enumerable grouping
                // case, where one iterates over the (outer) groups which then
                // can be iterated over for their elements. Substitute iterate
                // for subscribe and you got precisely this piece of code.
                //
                Console.WriteLine("Quadrant " + g.Key);
                g.Subscribe(x => Console.WriteLine(x + " in " + g.Key));
            });

            Application.Run(frm);
        }

        static void UpdateOnUISimple()
        {
            var frm = new Form();

            //
            // FromEvent bridges classic .NET events with IObservable<T>.
            // The result uses an IEvent<TEventArgs>, as specified on the generic parameter.
            //
            var mme = from evt in Observable.FromEvent<MouseEventArgs>(frm, "MouseMove")
                      select evt.EventArgs.Location;

            //
            // The reason the following works is simply because the input events were
            // generated on the UI thread. The OnNext lambda expression will be called
            // on that same thread.
            //
            // Notice that some operators introduce concurrency and will do so using some
            // scheduler. So we live in a free threaded world where the original event's
            // thread affinity is not necessarily retained. In the UpdateOnUITimer sample
            // we'll see how to hop to the right thread, e.g. to update a UI.
            //
            mme.Subscribe(loc => frm.Text = loc.ToString());

            Application.Run(frm);
        }

        static void UpdateOnUITimer()
        {
            var frm = new Form();

            //
            // Timer is one of the operators that generates events on a background thread.
            // Rx's way to introduce concurrency is an IScheduler, which can be passed to
            // all operators that have to do so. Defaults are chosen to be meaningful, e.g.
            // not to hijack the current thread during subscription, such that it's at all
            // possible to unsubscribe (by calling Dispose on the returned IDisposable).
            //
            // Use of the Timestamp operator turns an IObservable<T> into an observable
            // sequence with element type Timestamped<T>, containing a date and time. The
            // reverse operation is possible by using RemoveTimestamp. Here Rx differs from
            // StreamInsight in that Rx doesn't force time on events that have no intrinsic
            // semantics for time.
            //
            var sec = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1) /* optional scheduler */)
                      .Timestamp();

            //
            // Now we need ObserveOn to update the UI. When you drop the ObserveOn call and
            // run under the debugger, WinForms's MDA will detect a cross-thread access.
            //
            sec
                .ObserveOn(frm)
                .Subscribe(x => frm.Text = x.Timestamp.ToString());

            Application.Run(frm);
        }

        static void DelayedTyping()
        {
            //
            // A simple form with a textbox and a label adjacent to one another.
            //
            var txt = default(TextBox);
            var lbl = default(Label);
            var frm = new Form
            {
                Controls =
                {
                    (txt = new TextBox()),
                    (lbl = new Label { Left = txt.Width + 10 })
                }
            };

            //
            // Get TextChanges again using FromEvent, turned into an IObservable<string>.
            //
            var res = from txc in Observable.FromEvent<EventArgs /* useless event */>(txt, "TextChanged")
                      select ((TextBox)txc.Sender).Text;

            //
            // The Delay operator shifts events in time.
            //
            res
                .Delay(TimeSpan.FromSeconds(1))
                .ObserveOn(lbl) // Delay uses a timer, so we don't have the original UI thread affinity anymore
                .Subscribe(x => lbl.Text = x);

            Application.Run(frm);
        }

        static void Merge()
        {
            //
            // Two textboxes this time.
            //
            var tx1 = default(TextBox);
            var tx2 = default(TextBox);
            var frm = new Form
            {
                Controls =
                {
                    (tx1 = new TextBox()),
                    (tx2 = new TextBox() { Left = tx1.Left + tx1.Width + 10 })
                }
            };

            //
            // Using Merge we can turn multiple observable sequences into a single one. Events will come out
            // whenever any of the sources of the Merge operator signals a value.
            //
            var in1 = Observable.FromEvent<EventArgs>(tx1, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text);
            var in2 = Observable.FromEvent<EventArgs>(tx2, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text);

            using (in1.Merge(in2).Subscribe(text => Console.WriteLine(text)))
            {
                Application.Run(frm);
            } // Unsubscribes *both* handlers through composition.
        }

        static void TextBoxNoDuplicates()
        {
            //
            // C#, in JSON style :-).
            //
            var txt = default(TextBox);
            var frm = new Form
            {
                Controls = { (txt = new TextBox()) }
            };

            //
            // Let's get the TextChanged event using FromEvent, and use projection to
            // make it carry some interesting data. Remember that FromEvent produces
            // an observable sequence of IEvent<T> objects, which refer to a sender
            // and event arguments.
            //
            var chg = from txc in Observable.FromEvent<EventArgs /* useless event */>(txt, "TextChanged")
                      select ((TextBox)txc.Sender).Text;

            //
            // We'll append a side-effecting Do operator to log all of the occurrences
            // of a TextChanged event. For demo purposes, observe how the following
            // interaction sequence produces a redundant change notification:
            //
            //     Reactive     type this
            //       ^          select a
            //       a          type a     -->  TextChanged event is raised
            //
            var res = chg
                      .Do(str => Console.WriteLine("TextChanged: " + str))
                      .DistinctUntilChanged();

            //
            // To get rid of duplicates, we can use DistinctUntilChanged. The
            // interaction sequence shown above should no longer cause duplicates
            // on the output of the below.
            //
            // Notice the side-effecting Do above will still be triggered (as it's
            // used "upstream"), which clearly shows the issue.
            //
            res.Subscribe(Console.WriteLine);

            Application.Run(frm);
        }

        static void TextBoxWithThrottling()
        {
            //
            // The same code as above, now adding Throttle. This causes the event stream
            // to be calmed down, such that no values are produced if multiple events
            // occur within the specified timespan. Cf. IntelliSense only showing up info
            // after the user pauses typing for at least one second.
            //
            var txt = default(TextBox);
            var frm = new Form
            {
                Controls = { (txt = new TextBox()) }
            };

            //
            // Demo: type very fast and then stop for at least a second.
            //
            var chg = from txc in Observable.FromEvent<EventArgs /* useless event */>(txt, "TextChanged")
                      select ((TextBox)txc.Sender).Text;

            var res = chg
                      .DistinctUntilChanged()
                      .Throttle(TimeSpan.FromSeconds(1));

            res.Subscribe(text => Console.WriteLine(text));

            Application.Run(frm);
        }

        static void DictionarySuggestWebService()
        {
            //
            // Simple UI, asking the user to enter a word, which will be looked up in a
            // dictionary behind a web service.
            //
            var txt = default(TextBox);
            var lst = default(ListBox);
            var frm = new Form
            {
                Controls =
                {
                    (txt = new TextBox()),
                    (lst = new ListBox() {
                        Top = txt.Top + txt.Height + 10,
                        Height = 400,
                        Width = 200
                    })
                }
            };

            #region Service definition
            //
            // We mimick a web service which has varying response speeds. The variable
            // i is used to indicate the delay (in seconds) for the next request. After
            // each request, its value will be halved.
            //
            int i = 30;

            //
            // The (web) service is a function that takes a string and produces, in an
            // asynchronous manner, a series of answers. To keep things simple, we'll
            // consider the request of the service to be the characters of which the
            // input string consists. In reality, the output would different to reflect
            // the response type, e.g. IObservable<WordDefinition>.
            //
            Func<string, IObservable<char>> lookup = input =>
                Observable.Create<char>(obs =>
                {
                    Console.WriteLine("Call for: {0}", input);

                    //
                    // The input object is a string, which is an IEnumerable<char>,
                    // which can be turned into an observable using ToObservable.
                    //
                    // To simulate the delay of the service, we use the Delay operator,
                    // also side-effecting the value of i to be halved:
                    //
                    // - 1st request: 30 seconds
                    // - 2nd request: 15 seconds
                    // - 3rd request:  7 seconds
                    // - etc.
                    //
                    var sub = input.ToObservable()
                        .Delay(TimeSpan.FromSeconds(Math.Max(0, i /= 2)))
                        .Subscribe(obs);

                    return () =>
                    {
                        //
                        // We're in fact simply wrapping the observable seen above
                        // into another one (using Create) to allow us to inject
                        // some custom logging during unsubscribe.
                        //
                        Console.WriteLine("Unsubscribe!");
                        sub.Dispose();
                    };
                });
            #endregion

            //
            // Into business now. Let's get the TextChanged event and turn it into
            // an observable of entered string values. Using DistinctUntilChanged we're
            // avoiding duplicate consecutive text entries to trigger calls to our
            // web service. Finally, we wait till the user has stopped typing for
            // one second, before we hit the server.
            //
            var txtC = Observable.FromEvent<EventArgs>(txt, "TextChanged")
                       .Select(c => ((TextBox)c.Sender).Text)
                       .DistinctUntilChanged()
                       .Throttle(TimeSpan.FromSeconds(1));

            //
            // Here the magic happens, using SelectMany behind the scenes. Given the
            // input from the calmed down and filtered textbox, we make a call to the
            // the lookup service (cf. "lookup(input)" which returns an observable
            // sequence of results). The critical part is the use of TakeUntil, in
            // order to cancel out an in-flight request when a new request goes out.
            //
            // Try running the sample without TakeUntil on the second line of the
            // query comprehension, and follow the next steps:
            //
            // 1. Enter "Hello World" in one go (no pauses longer than 1 second)
            // 2. Wait till you see the web service print the request message
            // 3. Immediately replace "Hello World" by "Bang"
            // 4. Observe the second request being received by the web service
            //
            // Now the old request and the new request are in flight at the same
            // time, with the second one arriving before the first one (see the
            // service code on how we mimick this):
            //
            // txtC --"Hello World"-------------"Bang"------------------------...
            //                    <1s> |             <1s> |
            // svc                     +------------------|-------------'H'...
            //                                            +----'B'...
            //
            //                                                 Out of order arrival
            //
            // Simply by using TakeUntil we can cancel out the first request as soon
            // as a second text input is sent to the service:
            //
            // txtC --"Hello World"-------------"Bang"------------------------...
            //                    <1s> |             <1s> |
            // svc                     +------------------X
            //                                            +----'B'...
            //
            var res = from input in txtC
                      from word in lookup(input).TakeUntil(txtC)
                      select word;
            
            //
            // Alternative ways exist to achieve the same effect.
            // 
            // First of all, we can turn the above into the use of SelectMany,
            // which is used by the compiler to translate the fragment above.
            //
            var res1 = txtC
                       .SelectMany(input => lookup(input)
                                            .TakeUntil(txtC));

            //
            // Secondly, the Switch operator can be used in lieu of TakeUntil. It
            // operates on an IObservable<IObservable<T>>, producing results from
            // an inner observable till the outer one signals the availability of a
            // new one.
            //
            var res2 = txtC
                       .Select(input => lookup(input))
                       .Switch();

            //
            // Finally, the above can be simplified to the following, using eta-
            // reduction.
            //
            var res3 = txtC
                       .Select(lookup)
                       .Switch();

            //
            // "Bind" the incoming results to the ListBox control, one by one.
            //
            res.ObserveOn(lst).Subscribe(c =>
            {
                lst.Items.Add(c);
            });

            Application.Run(frm);
        }

        static void MaterializeAndBack()
        {
            //
            // Materialization and dematerialization transitions back and forth between the code
            // and data domain. An observer has three messages, OnNext, OnError and OnCompleted,
            // which are all about invoking code. Using Materialize an IObservable<T> can be
            // turned into an IObservable<Notification<T>> in order to analyze the notifications
            // as data objects, e.g. to filter on their type.
            //
            var src = Observable.Range(0, 10).Concat(Observable.Throw<int>(new Exception("Oops!")));

            //
            // Much like Fourier analysis, we have to do with two different domains in which we
            // can operate. Sometimes it's easier to carry out operations in one domain. After
            // we're done with the operation, we can go back to the original domain. Below is
            // something that looks quite a bit like the Catch operator.
            //
            var res = src
                      .Materialize()
                      .Where(not => not.Kind != NotificationKind.OnError)
                      .Dematerialize();

            //
            // Now the OnError message is gone.
            //
            res.Subscribe(
                Console.WriteLine,
                ex => Console.WriteLine("Should not happen"),
                () => Console.WriteLine("Done!")
            );

            //
            // Also look at the Catch, Finally, OnErrorResumeNext operators for exception handling
            // primitives over observable sequences.
            //
        }

        static void JoinPatterns()
        {
            //
            // A form with three buttons. When two buttons are clicked, we want to raise a message
            // telling which ones have been clicked. Whenever there are two click event messages
            // available, they should get combined. In other words, three observable sequences of
            // button clicks should be turned into a single observable of string messages.
            //
            var btn1 = default(Button);
            var btn2 = default(Button);
            var btn3 = default(Button);
            var frm = new Form
            {
                Controls = {
                    (btn1 = new Button { Text = "First" }),
                    (btn2 = new Button { Text = "Second", Left = 100 }),
                    (btn3 = new Button { Text = "Third", Left = 200 })
                }
            };

            //
            // The usual trick to bridge to regular .NET events. We're adding calls to the Do
            // operator to log the clicks.
            //
            var btn1Clicks = Observable.FromEvent<EventArgs>(btn1, "Click").Do(_ => Console.WriteLine("Button 1"));
            var btn2Clicks = Observable.FromEvent<EventArgs>(btn2, "Click").Do(_ => Console.WriteLine("Button 2"));
            var btn3Clicks = Observable.FromEvent<EventArgs>(btn3, "Click").Do(_ => Console.WriteLine("Button 3"));

            //
            // Observable.Join is much like WaitAny, while the use of And*.Then operator sequences
            // declares a so-called join pattern, much like WaitAll does. A large number of And
            // operator calls can be chained, followed by a Then which takes a lambda expression
            // that's given the data carried by the incoming events. In there, a projection is done
            // to the target data type. Compare this whole thing with Outlook rules.
            //
            //  INPUTS (n)             OUTPUT (1)
            //
            //  btn1.Click  ---+--------------------+
            //                 +-----> "1 and 2"    |
            //  btn2.Click  ---+-+     "1 and 3" <--+
            //                   +---> "2 and 3"    |
            //  btn3.Click  -----+------------------+
            //

            Func<IEvent<EventArgs>, IEvent<EventArgs>, string> resultSelector =
                (l, r) => ((Button)l.Sender).Text + " and " + ((Button)r.Sender).Text;

            Observable.Join( // WaitAny
                btn1Clicks.And(btn2Clicks) /* WaitAll */.Then(resultSelector),
                btn1Clicks.And(btn3Clicks).Then(resultSelector),
                btn2Clicks.And(btn3Clicks).Then(resultSelector)
            ).Subscribe(x => Console.WriteLine(x));

            Application.Run(frm);
            return;
        }

        static void Buffers()
        {
            //
            // The Timestamp operator tags time onto an IObservable<T>'s elements
            // resulting in an IObservable<Timestamped<T>>.
            //
            var src = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                      .Timestamp();

            //
            // Buffer operators take an IObservable<T> into an IObservable<IList<T>>,
            // either using an integer element count or a timespan. In addition, one
            // can supply skip lengths to allow overlapping or non-adjacent windows.
            //
            // We omit a sample of BufferWithTimeOrCount, the so-called ferry buffer,
            // which "leaves" when either the buffer is full (count) or a certain time
            // has elapsed.
            //
            var resT = src
                       .BufferWithTime(TimeSpan.FromSeconds(3));

            var resN = src
                       .BufferWithCount(3, /* skip */ 1);

            //
            // A little factory for OnNext handlers, printing the buffer with a title
            // indicating the type of buffering operator that has been used.
            //
            Func<string, Action<IList<Timestamped<long>>>> ctorOnNext = title => buffer =>
            {
                //
                // Since xs is an IList<T>, we could use LINQ to Objects in here...
                //
                Console.WriteLine("{0}: New buffer", title);
                foreach (var x in buffer)
                    Console.WriteLine(x);

                //
                // Here's an example of using LINQ to Objects on the buffer.
                //
                Console.WriteLine(buffer.Select(ts => ts.Value).Average());
            };

            using (resT.Subscribe(ctorOnNext("T")))
            using (resN.Subscribe(ctorOnNext("N")))
            {
                Thread.Sleep(10000);
            }
        }

        static void Aggregates()
        {
            //
            // Aggregation operators turn a multi-element sequence into a single
            // value. In the world of LINQ to Objects, those operators have a
            // return type outside the monad:
            //
            //   static int Max(this IEnumerable<int> source);
            //
            // Unfortunately, this doesn't allow for further composition on the
            // result, e.g. to specify a Timeout operator or so. In other words,
            // those operators are blocking, which clearly clashes with our goal
            // of asynchronous programming. In the world of IObservable<T>, we
            // stay in the monad instead:
            //
            //   static IObservable<int> Max(this IObservable<int> source);
            //
            // Sometimes people debate the distinction between "one versus many"
            // and whether the arity of a sequence should be visible in the type.
            // This creates quite a complicated world with lots of duplication
            // of operators (e.g. a Select operator would preserve arity). We
            // don't got there and simply say that an IObservable<T> represents
            // zero or more elements, just like an IEnumerable<T> does. The case
            // of "one" is simply a special case of "many".
            //
            var src = Observable.GenerateWithTime(1, i => i <= 10, i => i, i => TimeSpan.FromMilliseconds(i * 50), i => i + 1);

            //
            // Publish is used to share an observable sequence to many consumers
            // inside the lambda expression.
            //
            src.Publish(xs =>
            {
                //
                // Notice none of the below are blocking, so we're effectively
                // computing the aggregates in parallel.
                //
                xs.Min().Subscribe(x => Console.WriteLine("Min = " + x));
                xs.Max().Subscribe(x => Console.WriteLine("Max = " + x));
                xs.Sum().Subscribe(x => Console.WriteLine("Sum = " + x));
                xs.Count().Subscribe(x => Console.WriteLine("Count = " + x));
                xs.Average().Subscribe(x => Console.WriteLine("Average = " + x));
                xs.Aggregate(1L, (prd, x) => prd * x).Subscribe(x => Console.WriteLine("Product = " + x));

                //
                // As an extension on LINQ to Objects, we also provide *By
                // variants for Min and Max, allowing use of a key selector.
                // For other aggregates like Sum, one can use Select before
                // carrying out the aggregate.
                //
                xs.MinBy(x => -x).Subscribe(x => Console.WriteLine("MinBy(-) = " + x));
                xs.MaxBy(x => -x).Subscribe(x => Console.WriteLine("MaxBy(-) = " + x));

                //
                // Some aggregates don't need to wait till all elements have
                // been observed. Some of the below will fire as soon as an
                // answer is known.
                //
                xs.Any(x => x > 5).Subscribe(b => Console.WriteLine("Any > 5 = " + b));
                xs.Any(x => x < 0).Subscribe(b => Console.WriteLine("Any < 0 = " + b));
                xs.All(x => x > 0).Subscribe(b => Console.WriteLine("All > 0 = " + b));
                xs.All(x => x < 5).Subscribe(b => Console.WriteLine("All < 5 = " + b));
                xs.Contains(0).Subscribe(b => Console.WriteLine("Contains 0 = " + b));
                xs.Contains(5).Subscribe(b => Console.WriteLine("Contains 5 = " + b));
                
                //
                // First is an exception to the rule. It serves a decomposition
                // nature for a sequence into a head and a tail along the lines
                // of LISP's cons, car and cdr. To observe the first element as
                // an observable sequence, one can use Take(1). To observe the
                // tail, one can use Skip(1).
                //
                // The signature of First (and Last as well) follows LINQ to
                // Objects and leaves the monad. The TakeLast operator can be
                // used to stay in the monad and observe the last element.
                //
                xs.Take(1).Subscribe(x => Console.WriteLine("First = " + x));
                xs.TakeLast(1).Subscribe(x => Console.WriteLine("Last = " + x));
                
                return xs;
            }).Run(Console.WriteLine);
        }

        static void Bridges()
        {
            var evt = new MyEvents();

            //
            // Rx doesn't claim to substitue existing technologies with some new
            // approach using IObservable exclusively. For example, regular .NET
            // events, the asynchronous pattern, etc. are still perfectly fine to
            // be used. However, when composition enters the picture, Rx provides
            // a solution. Therefore we can bridge the old world to the new world
            // using various operators. Rx is the glue that binds those worlds
            // together using operators and combinators.
            //

            //
            // A first sample is FromEvent for which we have a reflective version
            // as well as one that uses add/remove handler delegates.
            //
            Console.WriteLine("FromEvent");
            var mme1 = Observable.FromEvent<MyEventArgs>(evt, "Bar");
            var mme2 = Observable.FromEvent<MyEventArgs>(handler => evt.Bar += handler, handler => evt.Bar -= handler);

            using (mme1.Subscribe(e => Console.WriteLine("1 --> " + e.EventArgs.Value)))
            using (mme2.Subscribe(e => Console.WriteLine("2 --> " + e.EventArgs.Value)))
            {
                evt.Raise(42);
                evt.Raise(43);
            }

            evt.Raise(24); // All subscriptions gone; no output expected.
            Console.WriteLine();

            //
            // Secondly, there's the asynchronous invocation Begin/End pair pattern,
            // which can be bridged using FromAsyncPattern.
            //
            Console.WriteLine("FromAsyncPattern");
            using (var fs = File.OpenRead(@"..\..\Program.cs"))
            {
                //
                // The signature of the Begin method is reflected in the FromAsyncPattern
                // generic parameters. The last one is the return type, in this case an
                // int returning the number of bytes read, as seen on the End method.
                //
                var readAsync = Observable.FromAsyncPattern<byte[], int, int, int>(fs.BeginRead, fs.EndRead);

                //
                // Using readAsync, we can invoke the Begin/End pair without worrying about
                // the IAsyncResult madness. In here, res will be an IObservable<int> which
                // will contain the result of the EndRead call, i.e. the number of bytes
                // read from the input.
                //
                var data = new byte[16];
                var res = readAsync(data, 0, data.Length);

                //
                // We'll use Run to go synchronously here, such that we don't dispose the
                // opened FileStream prematurely.
                //
                res.Run(x => Console.WriteLine("{0} bytes read: {1}", x, Encoding.ASCII.GetString(data)));
            }
            Console.WriteLine();

            //
            // Third, the Task Parallel Library introduced in .NET 4 (and backported to 3.5
            // when installing Rx) has the notion of a Task<T> which is like a single-result
            // IObservable<T> (i.e. a deferred Return<T> observable). We can convert this to
            // an observable as well.
            //
            Console.WriteLine("Task<T>");
            var tsk = Task<int>.Factory.StartNew(() => { Thread.Sleep(2000); return 42; });
            tsk.ToObservable().Run(Console.WriteLine);
            Console.WriteLine();

            //
            // Fourth, enumerable sequences can be iterated asynchronously, resulting in an
            // observable sequence as well. Let's create some sequence that ticks at a slow
            // pace, producing values 0..9, and watch it using observables. Instead of using
            // Run which would make the whole thing synchronous, we're printing dots on the
            // main thread till completion is signaled by the enumeration that's happening
            // on a background thread spawn by ToObservable. This illustrates how concurrency
            // is introduced by ToObservable.
            //
            Console.WriteLine("ToObservable");
            var xs = Enumerable.Range(0, 10).Select(x => { Thread.Sleep(x * 100); return x; });
            var isDone = false;
            xs.ToObservable(Scheduler.ThreadPool /* where to do the iteration */)
              .Subscribe(Console.WriteLine, () => isDone = true);
            while (!isDone)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }
            Console.WriteLine();

            //
            // Finally, regular functions or actions can be invoked in the background as well,
            // exposing their results through an IObservable using the Start operator.
            //
            Console.WriteLine("Func");
            var anw = Observable.Start(() => { Thread.Sleep(2000); return 42; });
            anw.Run(Console.WriteLine);
            Console.WriteLine();
        }

        class MyEvents
        {
            public event EventHandler<MyEventArgs> Bar;

            public void Raise(int value)
            {
                var bar = Bar;
                if (bar != null)
                    Bar(this, new MyEventArgs(value));
            }
        }

        class MyEventArgs : EventArgs
        {
            public MyEventArgs(int value)
            {
                Value = value;
            }

            public int Value { get; private set; }
        }

        static void Exceptions()
        {
            //
            // Observable sequences can signal three messages, as seen on the IObserver<T>
            // interface. One of those is OnError, indicating the presence of an error by
            // means of an Exception object. Operators that deal with exceptions exist in
            // Rx, allowing a compositional view on error-carrying sequences.
            //
            
            //
            // Let's start by looking at the Catch operator which is the equivalent to:
            //
            // try {
            //    some sequence producing results
            // } catch (Exception ex) {
            //    fallback sequence to continue with upon an exception of the given type
            // }
            //
            // We'll start by generating an infinite sequence of random numbers.
            //
            var rnd = new Random();
            var inp = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                      .Select(_ => rnd.Next(0, 5))
                      .Do(x => Console.WriteLine("Lottery --> " + x));

            //
            // Now let's create a sequence that exposes an error sometimes.
            //
            var res = inp.Select(x => 1000 / x);

            //
            // The OnError handler will be triggered for division-by-zero exceptions if
            // we simply subscribe. If we apply the Catch operator, we can specify a
            // fallback sequence. In addition, n-ary overloads exist.
            //
            res.Publish(xs =>
            {
                //
                // Use of Publish allows us to share the underlying subscription to the
                // observable sequence. In the lambda body we can subscribe to "res" by
                // using the exposed "xs" parameter.
                //
                // Here we use Catch to catch the DivideByZeroException and return a new
                // observable in that case. For illustration purposes we simply use the
                // Return operator.
                //
                var cth = xs.Catch<int, DivideByZeroException>(ex => Observable.Return(42));
                cth.Subscribe(x => Console.WriteLine("cth --> " + x));

                //
                // We leak the input to the outside in a verbatim manner, so we can also
                // subscribe from the outside. Other overloads to Publish exist that can
                // be used to expose an observable sequence as an IConnectableObservable
                // object. This allows for staged subscription:
                //
                //   var xs = res.Publish();
                //   xs.(operators).Subscribe(...);
                //   xs.(operators).Subscribe(...);
                //   xs.Connect(); // Now the sequences composed above will be connected
                //                 // to the underlying xs sequence.
                //
                return xs;
            }).Run(
                x  => Console.WriteLine("res --> " + x),
                ex => Console.WriteLine("res --> " + ex.Message)
            );

            //
            // Other operators in the exception handling camp include OnErrorResumeNext
            // which is similar to applying Concat between different sequences, but it
            // does move on to the next sequence even when the previous one terminated
            // with an error. The Finally operator is the equivalent to C#'s finally
            // block, and Using corresponds to the using keyword used in proper resource
            // disposal.
            //
        }

        static void Sequential()
        {
            //
            // Sequential "statement" operators exist, which correspond to the well-known
            // statements supported in languages like C#. One sample is If, which selects
            // between two observable sequences based on a Func<bool>. This selector gets
            // invoked upon subscription to the resulting sequence.
            //
            // In the sample below we also illustrate the While operator, which subscribes
            // to the given observable sequence again based on the evaluation result of
            // the condition that's passed in. Conceptually this is the same as a Repeat
            // operation that has a terminating condition. In here, n is incremented in
            // the OnNext handler for every received element.
            //
            // As an exercise, play with the DoWhile operator as well.
            //
            int n = 0;
            Observable.While(
                () => n < 10,
                Observable.If(() =>
                    {
                        string res = "";

                        do
                        {
                            Console.Write("Enter E for even, O for odd: ");
                            res = Console.ReadLine().ToLowerInvariant();
                        } while (res != "e" && res != "o");

                        return res == "e";
                    },
                    Observable.Range(0, 5).Select(x => x * 2),
                    Observable.Range(0, 5).Select(x => x * 2 + 1)
                )
            )
            .Run(x =>
            {
                Console.WriteLine(x);
                n++;
            });
        }

        static void MoreOnTime()
        {
            //
            // The notion of time is very tightly integrated with Rx, to an extent that
            // core facilities like System.Concurrency.IScheduler have this very notion
            // built-in. The use of time also surfaces in various operators.
            //
            var lbl = new Label { Text = ".", Font = new Font("Consolas", 20, FontStyle.Bold), AutoSize = true };
            var frm = new Form { Controls = { lbl } };

            var mme = Observable.FromEvent<MouseEventArgs>(frm, "MouseMove").Select(mm => mm.EventArgs.Location);

            //
            // Let the dot follow the mouse cursor, but delayed one second.
            //
            var d1 = mme.Delay(TimeSpan.FromSeconds(1)).Subscribe(pt => lbl.Location = pt);

            //
            // Sample where the mouse cursor is once every second.
            //
            var d2 = mme.Sample(TimeSpan.FromSeconds(1)).Subscribe(pt => Console.WriteLine("Sampled: " + pt));

            //
            // Throttle will require a second of inactivity to forward a message.
            //
            var d3 = mme.Throttle(TimeSpan.FromSeconds(1)).Subscribe(pt => Console.WriteLine("Throttled: " + pt));

            //
            // Analysis over buffers, with a sliding window analyzing 5 seconds and
            // moving 2 seconds every time:
            //
            // 0123456789
            // -----
            //   -----
            //     -----
            //       ----
            //
            var d4 = mme.BufferWithTime(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2)).Subscribe(pts => Console.WriteLine("# of places been: " + pts.Count));

            //
            // System.Disposables contains an "algebra over IDisposable". For example,
            // the CompositeDisposable allows multiple disposables to be disposed in
            // a group. It's like the sum operator for IDisposables.
            //
            using (new CompositeDisposable(d1, d2, d3, d4))
            {
                Application.Run(frm);
            }
        }

        //
        // More samples to follow on other operators, the use of schedulers, etc.
        //
    }
}
