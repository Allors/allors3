import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute } from '@angular/router';

import { BehaviorSubject, Subscription, combineLatest } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ErrorService, Invoked, SessionService, NavigationActivatedRoute, NavigationService, MetaService } from '../../../../../angular';
import { PullRequest } from '../../../../../framework';
import { MetaDomain } from '../../../../../meta';
import { AllorsMaterialDialogService } from '../../../../base/services/dialog';
import { Title } from '@angular/platform-browser';

import { EmailCommunication, Party, WorkTask } from '../../../../../domain';

@Component({
  templateUrl: './emailcommunication-overview.component.html',
  providers: [SessionService]
})
export class EmailCommunicationOverviewComponent implements OnInit, OnDestroy {

  public title = 'Communication Event overview';
  public m: MetaDomain;

  public party: Party;
  public emailCommunication: EmailCommunication;

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;

  constructor(
    @Self() private allors: SessionService,
    public metaService: MetaService,
    public navigationService: NavigationService,
    private errorService: ErrorService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialogService: AllorsMaterialDialogService,
    titleService: Title) {

    titleService.setTitle(this.title);

    this.m = this.metaService.m;
    this.refresh$ = new BehaviorSubject<Date>(undefined);
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    this.subscription = combineLatest(this.route.url, this.refresh$)
      .pipe(
        switchMap(([urlSegments, date]) => {

          const navRoute = new NavigationActivatedRoute(this.route);
          const id = navRoute.id();
          const partyId = navRoute.queryParam(m.Party);

          const pulls = [
            pull.Party({ object: partyId }),
            pull.EmailCommunication({
              object: id,
              include: {
                Originator: x,
                Addressees: x,
                EmailTemplate: x,
                EventPurposes: x,
                CommunicationEventState: x,
                ContactMechanisms: x,
                WorkEfforts: {
                  WorkEffortState: x,
                  Priority: x,
                }
              }
            })
          ];

          return this.allors
            .load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.allors.session.reset();

        this.party = loaded.objects.Party as Party;
        this.emailCommunication = loaded.objects.EmailCommunication as EmailCommunication;
      }, this.errorService.handler);
  }

  public deleteWorkEffort(worktask: WorkTask): void {

    this.dialogService
      .confirm({ message: 'Are you sure you want to delete this work task?' })
      .subscribe((confirm: boolean) => {
        if (confirm) {
          this.allors.invoke(worktask.Delete)
            .subscribe((invoked: Invoked) => {
              this.snackBar.open('Successfully deleted.', 'close', { duration: 5000 });
              this.refresh();
            },
              (error: Error) => {
                this.errorService.handle(error);
              });
        }
      });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public goBack(): void {
    window.history.back();
  }
}
