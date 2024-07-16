DrawSlices3=DrawSlices2(:);
canvas = PlotLines(allSlices(DrawSlices3),bestSlice);
graymap(canvas);

canvas2=canvas(:,:);
idx=find(canvas~=0);
    
for K=1:1
    hh = fspecial('average', 5);
    for I=1:50
        
        canvas2=imfilter(canvas2,hh);
        %canvas2=doBM3DImageFilter(canvas2);
        canvas2(idx)=canvas(idx);
        imagesc(canvas2);drawnow;
    end
    hh = fspecial('average', 3);
    for I=1:50
        canvas2=imfilter(canvas2,hh);
        %canvas2=doBM3DImageFilter(canvas2);
        canvas2(idx)=canvas(idx);
        imagesc(canvas2);drawnow;
    end
%     for I=1:3
%         canvas2(idx)=canvas(idx);
%         % canvas2=imfilter(canvas2,hh);
%         canvas2=doBM3DImageFilter(canvas2);
%         
%         imagesc(canvas2);drawnow;
%     end
     imagesc(canvas2(80:280,80:280));drawnow;
   % quality=TestLines(allSlices,bestSlice,canvas2,X-L/2,Y-L/2);
   % DrawSlices3 = find(quality<2000);
   % canvas = PlotLines(allSlices(DrawSlices3),bestSlice);
end
canvas=medfilt2(canvas2,[5 5]);

L=size(slices{1}.array,2);

X=0;
Y=0;
cc=0;
for I=1:length(slices)
    if isfield(slices{I},'Point1')==true && slices{I}.AlreadyDone==true
        [x y]=findPoint(slices{I}, [1 L]);
        xx=x(1)+x(2);
        yy=y(1)+y(2);
        if isnan(xx)==false && isinf(xx)==false && isnan(yy)==false && isinf(yy)==false
            X=X+x(1)+x(2);
            Y=Y+y(1)+y(2);
            cc=cc+2;
        end
    end
end
X=X/cc;
Y=Y/cc;

minX=X-L/2;
minY=Y-L/2;
maxX=X+L/2;
maxY=Y+L/2;


ccc=1;
quality=zeros([length(DrawSlices3) 1]);
for I=1:length(DrawSlices3)
    
    if isfield(allSlices{DrawSlices3(I)},'Point1')==true
        A1=allSlices{DrawSlices3(I)}.array(:,bestSlice);
        
        [x y]=findPoint(allSlices{DrawSlices3(I)}, 1:L);
        s=0;
        cc=0;
        for J=1:L
            i=round(x(J)-minX);
            j=round(y(J)-minY);
            if (isnan(i)==false && isnan(j)==false && isinf(i)==false && isinf(j)==false)
                if (i>100 && j>100 && i<L-100 &&j<L-100)
                    c=canvas(i,j);
                    if c~=0
                        s=s+(c - A1(J))^2;
                        cc=cc+1;
                    end
                end
            end
        end
        quality(I)=(s/(cc-1))^.5;
        ccc=ccc+1
    else 
        quality(I)=10000000;
    end
end
quality(quality==0)=10000000;
canvasQuality=mean(quality(quality~=10000000));
sd=std(quality(quality~=10000000));
canvasQuality=canvasQuality+ sd*1.3
DrawSlices4=DrawSlices3(quality<(canvasQuality));

standCanvas=canvas(:,:);


canvas = PlotLines(allSlices(DrawSlices4),bestSlice);
graymap(canvas);
% badSlices = find(quality>1500);
% 
% chosen=randperm(length(DrawSlices2));
% 
% chosen =DrawSlices2(chosen);
% slice1=allSlices{chosen(1)};
% slice2=allSlices{chosen(2)};
% slice3=allSlices{chosen(3)};
% 
% 
% slices={slice1,slice2,slice3};
% 
% DrawSlices=chosenTriad(:)';
% for J=1:length(badSlices)
%    I=badSlices(J);
%         [idx1_T idxT_1 minVal1]= findCross(slice1,allSlices{I},maxM);
%         [idx2_T idxT_2 minVal2]= findCross(slice2,allSlices{I},maxM);
%         [idx3_T idxT_3 minVal3]= findCross(slice3,allSlices{I},maxM);
%         
%         indexS=[idx1_T idx2_T idx3_T];
%         indexT=[idxT_1 idxT_2 idxT_3];
%         
%         [m idx]=sort([minVal1 minVal2 minVal3]);
%         indexS=indexS(idx);
%         indexT=indexT(idx);
%         
%         if m(1)<goodHit
%             [x y]=findPoint(slices{idx(1)},indexS(1));
%             P1=[x y indexT(1)];
%             [x y]=findPoint(slices{idx(2)},indexS(2));
%             P2=[x y indexT(2)];
%             
%             if (abs(P1(1)-P2(1))>5 && abs(P1(2)-P2(2))>5 && abs(P1(3)-P2(3))>6)
%                 
%                 allSlices{I}.Point1=P1;
%                 allSlices{I}.Point2=P2;
%                 allSlices{I}.AlreadyDone=true;
%                 DrawSlices=[DrawSlices, I];
%             end
%         end
%   
% end